using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using pulaVocab.Application.Practice;
using pulaVocab.Domain.Entities;
using pulaVocab.Domain.Enums;

namespace pulaVocab.Infrastructure;

public sealed class PracticeService(VocabMasterDbContext db, SpacedRepetitionService repetition) : IPracticeService
{
    public async Task<PracticeStatisticsResponse> GetStatisticsAsync(Language language, CancellationToken ct)
    {
        var now = DateTime.UtcNow; var words = db.VocabularyEntries.AsNoTracking().Where(x => x.Language == language && x.Meanings.Any());
        var total = await words.CountAsync(ct); var progress = db.WordPracticeProgress.AsNoTracking().Where(x => x.VocabularyEntry!.Language == language);
        var recent = db.PracticeAnswers.AsNoTracking().Where(x => x.VocabularyEntry!.Language == language).OrderByDescending(x => x.AnsweredAtUtc).Take(100);
        var recentCount = await recent.CountAsync(ct); var correct = await recent.CountAsync(x => x.IsCorrect, ct);
        return new() { Total = total, New = total - await progress.CountAsync(ct), DueToday = await progress.CountAsync(x => x.NextReviewAtUtc <= now, ct), Difficult = await progress.CountAsync(x => x.MasteryLevel == MasteryLevel.Difficult, ct), Mastered = await progress.CountAsync(x => x.MasteryLevel == MasteryLevel.Mastered, ct), RecentAccuracy = recentCount == 0 ? 0 : Math.Round(correct * 100d / recentCount, 1), Levels = await words.Where(x => x.Level != null).Select(x => x.Level!.Value).Distinct().OrderBy(x => x).ToListAsync(ct), PartsOfSpeech = await words.Where(x => x.PartOfSpeech != null).Select(x => x.PartOfSpeech!.Value).Distinct().OrderBy(x => x).ToListAsync(ct) };
    }
    public async Task<IReadOnlyList<PracticeWordResponse>> PreviewAsync(PracticeFilterRequest filter, CancellationToken ct) => (await Select(filter, ct)).Select(Map).ToList();
    public async Task<PracticeSessionResponse> StartAsync(StartPracticeRequest request, CancellationToken ct)
    {
        var words = await Select(request.Filter, ct); if (request.ExerciseType == PracticeExerciseType.Matching) words = MatchingRoundBuilder.RemoveAmbiguous(words.Select(Map)).Select(x => words.Single(w => w.Id == x.Id)).ToList();
        var session = new PracticeSession(request.Filter.Language, request.ExerciseType, words.Count, JsonSerializer.Serialize(request.Filter), DateTime.UtcNow, request.ParentSessionId); db.Add(session); await db.SaveChangesAsync(ct); return Map(session, words);
    }
    public async Task<PracticeSessionResponse?> SubmitAsync(Guid id, SubmitPracticeAnswerRequest request, CancellationToken ct)
    {
        var session = await db.PracticeSessions.FirstOrDefaultAsync(x => x.Id == id, ct); if (session is null || session.Status != PracticeSessionStatus.InProgress) return null;
        var word = await db.VocabularyEntries.FirstOrDefaultAsync(x => x.Id == request.VocabularyEntryId && x.Language == session.Language, ct); if (word is null) throw new InvalidOperationException("La palabra ya no está disponible.");
        if (await db.PracticeAnswers.AnyAsync(x => x.PracticeSessionId == id && x.VocabularyEntryId == request.VocabularyEntryId && x.AttemptNumber == request.AttemptNumber, ct)) return await Get(id, ct);
        var correct = session.ExerciseType == PracticeExerciseType.Flashcards ? request.Rating != PracticeRating.Forgot : request.IsCorrect;
        db.Add(new PracticeAnswer(id, word.Id, session.ExerciseType, request.Answer, correct, request.AttemptNumber, request.ResponseTimeMilliseconds, DateTime.UtcNow)); session.Register(correct);
        var progress = await db.WordPracticeProgress.SingleOrDefaultAsync(x => x.VocabularyEntryId == word.Id, ct) ?? new WordPracticeProgress(word.Id); if (db.Entry(progress).State == EntityState.Detached) db.Add(progress);
        repetition.Apply(progress, session.ExerciseType == PracticeExerciseType.Matching ? (correct ? PracticeRating.Good : PracticeRating.Forgot) : request.Rating!.Value, session.ExerciseType, DateTime.UtcNow);
        await db.SaveChangesAsync(ct); return await Get(id, ct);
    }
    public async Task<PracticeSessionResponse?> FinishAsync(Guid id, FinishPracticeRequest request, CancellationToken ct) { var session = await db.PracticeSessions.FirstOrDefaultAsync(x => x.Id == id, ct); if (session is null) return null; if (session.Status == PracticeSessionStatus.InProgress) { session.Finish(DateTime.UtcNow, request.EndedEarly); await db.SaveChangesAsync(ct); } return await Get(id, ct); }
    private async Task<List<VocabularyEntry>> Select(PracticeFilterRequest f, CancellationToken ct)
    {
        var now = DateTime.UtcNow; var q = db.VocabularyEntries.Include(x => x.Meanings).Include(x => x.Examples).Include(x => x.EnglishDetails).Include(x => x.GermanDetails).Include(x => x.PracticeProgress).Include(x => x.VocabularyEntryTags).ThenInclude(x => x.VocabularyTag).Where(x => x.Language == f.Language && x.Meanings.Any());
        if (f.Level != null) q = q.Where(x => x.Level == f.Level); if (f.PartOfSpeech != null) q = q.Where(x => x.PartOfSpeech == f.PartOfSpeech); if (f.FavoritesOnly || f.State == "favorites") q = q.Where(x => x.VocabularyEntryTags.Any(t => t.VocabularyTag!.Name.ToLower() == "favorita" || t.VocabularyTag.Name.ToLower() == "favorite"));
        q = f.State switch { "new" => q.Where(x => x.PracticeProgress == null), "difficult" => q.Where(x => x.PracticeProgress!.MasteryLevel == MasteryLevel.Difficult), "due" => q.Where(x => x.PracticeProgress!.NextReviewAtUtc <= now), "mastered" => q.Where(x => x.PracticeProgress!.MasteryLevel == MasteryLevel.Mastered), _ => q };
        if (f.WordIds is { Count: > 0 }) q = q.Where(x => f.WordIds.Contains(x.Id));
        return await q.OrderBy(x => x.PracticeProgress == null ? 2 : x.PracticeProgress.NextReviewAtUtc <= now ? 0 : x.PracticeProgress.MasteryLevel == MasteryLevel.Difficult ? 1 : 3).ThenBy(x => Guid.NewGuid()).Take(Math.Clamp(f.Count, 1, 20)).ToListAsync(ct);
    }
    private async Task<PracticeSessionResponse?> Get(Guid id, CancellationToken ct) { var s = await db.PracticeSessions.AsNoTracking().Include(x => x.Answers).FirstOrDefaultAsync(x => x.Id == id, ct); return s is null ? null : Map(s, []); }
    private static PracticeSessionResponse Map(PracticeSession s, IEnumerable<VocabularyEntry> words) => new() { Id=s.Id, Language=s.Language, ExerciseType=s.ExerciseType, Status=s.Status, StartedAtUtc=s.StartedAtUtc, ScheduledCount=s.ScheduledCount, PracticedCount=s.PracticedCount, CorrectCount=s.CorrectCount, IncorrectCount=s.IncorrectCount, DurationMilliseconds=s.DurationMilliseconds, Words=words.Select(Map).ToList(), IncorrectWordIds=s.Answers.Where(x=>!x.IsCorrect).Select(x=>x.VocabularyEntryId).Distinct().ToList() };
    private static PracticeWordResponse Map(VocabularyEntry x) => new() { Id=x.Id, Term=x.Language==Language.German && !string.IsNullOrWhiteSpace(x.GermanDetails?.Article) ? $"{x.GermanDetails.Article} {x.Term}" : x.Term, Language=x.Language, PartOfSpeech=x.PartOfSpeech, Level=x.Level, Pronunciation=x.Pronunciation, Ipa=x.Ipa, IpaAmerican=x.IpaAmerican, IpaBritish=x.IpaBritish, MainTranslation=x.Meanings.OrderBy(m=>m.DisplayOrder).First().Translation, Meanings=x.Meanings.OrderBy(m=>m.DisplayOrder).Select(m=>new PracticeMeaningResponse{Translation=m.Translation,Definition=m.Definition,Context=m.Context}).ToList(), Examples=x.Examples.Select(e=>new PracticeExampleResponse{Sentence=e.Sentence,Translation=e.Translation}).ToList(), Synonyms=x.Synonyms, RelatedTerms=x.RelatedTerms, EnglishDetails=x.EnglishDetails is null?null:new(){Infinitive=x.EnglishDetails.Infinitive,PastTense=x.EnglishDetails.PastTense,PastParticiple=x.EnglishDetails.PastParticiple,ThirdPersonSingular=x.EnglishDetails.ThirdPersonSingular,Gerund=x.EnglishDetails.Gerund,RelatedPhrasalVerbs=x.EnglishDetails.RelatedPhrasalVerbs}, GermanDetails=x.GermanDetails is null?null:new(){Gender=x.GermanDetails.Gender?.ToString(),Article=x.GermanDetails.Article,Plural=x.GermanDetails.Plural,AuxiliaryVerb=x.GermanDetails.AuxiliaryVerb,PastParticiple=x.GermanDetails.PastParticiple,IsSeparableVerb=x.GermanDetails.IsSeparableVerb,SeparablePrefix=x.GermanDetails.SeparablePrefix} };
}
