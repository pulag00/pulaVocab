using pulaVocab.Domain.Enums;

namespace pulaVocab.Application.Practice;

public sealed class PracticeFilterRequest { public Language Language { get; set; } public int Count { get; set; } = 10; public CefrLevel? Level { get; set; } public PartOfSpeech? PartOfSpeech { get; set; } public string State { get; set; } = "all"; public bool FavoritesOnly { get; set; } public List<Guid>? WordIds { get; set; } }
public sealed class PracticeStatisticsResponse { public int Total { get; set; } public int New { get; set; } public int DueToday { get; set; } public int Difficult { get; set; } public int Mastered { get; set; } public double RecentAccuracy { get; set; } public List<CefrLevel> Levels { get; set; } = []; public List<PartOfSpeech> PartsOfSpeech { get; set; } = []; }
public sealed class PracticeWordResponse
{
    public Guid Id { get; set; } public string Term { get; set; } = ""; public Language Language { get; set; } public PartOfSpeech? PartOfSpeech { get; set; } public CefrLevel? Level { get; set; }
    public string? Pronunciation { get; set; } public string? Ipa { get; set; } public string? IpaAmerican { get; set; } public string? IpaBritish { get; set; } public string MainTranslation { get; set; } = "";
    public List<PracticeMeaningResponse> Meanings { get; set; } = []; public List<PracticeExampleResponse> Examples { get; set; } = []; public string? Synonyms { get; set; } public string? RelatedTerms { get; set; }
    public PracticeEnglishDetails? EnglishDetails { get; set; } public PracticeGermanDetails? GermanDetails { get; set; }
}
public sealed class PracticeMeaningResponse { public string Translation { get; set; } = ""; public string? Definition { get; set; } public string? Context { get; set; } }
public sealed class PracticeExampleResponse { public string Sentence { get; set; } = ""; public string Translation { get; set; } = ""; }
public sealed class PracticeEnglishDetails { public string? Infinitive { get; set; } public string? PastTense { get; set; } public string? PastParticiple { get; set; } public string? ThirdPersonSingular { get; set; } public string? Gerund { get; set; } public string? RelatedPhrasalVerbs { get; set; } }
public sealed class PracticeGermanDetails { public string? Gender { get; set; } public string? Article { get; set; } public string? Plural { get; set; } public string? AuxiliaryVerb { get; set; } public string? PastParticiple { get; set; } public bool IsSeparableVerb { get; set; } public string? SeparablePrefix { get; set; } }
public sealed class StartPracticeRequest { public PracticeFilterRequest Filter { get; set; } = new(); public PracticeExerciseType ExerciseType { get; set; } public Guid? ParentSessionId { get; set; } }
public sealed class PracticeSessionResponse { public Guid Id { get; set; } public Language Language { get; set; } public PracticeExerciseType ExerciseType { get; set; } public PracticeSessionStatus Status { get; set; } public DateTime StartedAtUtc { get; set; } public int ScheduledCount { get; set; } public int PracticedCount { get; set; } public int CorrectCount { get; set; } public int IncorrectCount { get; set; } public long? DurationMilliseconds { get; set; } public List<PracticeWordResponse> Words { get; set; } = []; public List<Guid> IncorrectWordIds { get; set; } = []; }
public sealed class SubmitPracticeAnswerRequest { public Guid VocabularyEntryId { get; set; } public string Answer { get; set; } = ""; public PracticeRating? Rating { get; set; } public bool IsCorrect { get; set; } public int AttemptNumber { get; set; } = 1; public long? ResponseTimeMilliseconds { get; set; } }
public sealed class FinishPracticeRequest { public bool EndedEarly { get; set; } }

public interface IPracticeService
{
    Task<PracticeStatisticsResponse> GetStatisticsAsync(Language language, CancellationToken ct);
    Task<IReadOnlyList<PracticeWordResponse>> PreviewAsync(PracticeFilterRequest filter, CancellationToken ct);
    Task<PracticeSessionResponse> StartAsync(StartPracticeRequest request, CancellationToken ct);
    Task<PracticeSessionResponse?> SubmitAsync(Guid sessionId, SubmitPracticeAnswerRequest request, CancellationToken ct);
    Task<PracticeSessionResponse?> FinishAsync(Guid sessionId, FinishPracticeRequest request, CancellationToken ct);
}
