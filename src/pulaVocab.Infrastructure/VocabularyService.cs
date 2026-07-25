using Microsoft.EntityFrameworkCore;
using pulaVocab.Application.Vocabulary;
using pulaVocab.Domain.Entities;
using pulaVocab.Domain.Enums;

namespace pulaVocab.Infrastructure;

public sealed class VocabularyService(VocabMasterDbContext db) : IVocabularyService
{
    public async Task<PagedResponse<VocabularyListItemResponse>> GetPagedAsync(VocabularyFilterRequest filter, CancellationToken cancellationToken)
    {
        var page = Math.Max(1, filter.Page);
        var size = Math.Clamp(filter.PageSize, 1, 100);
        var query = db.VocabularyEntries.AsNoTracking().Include(x => x.Meanings).AsQueryable();
        if (filter.Language.HasValue) query = query.Where(x => x.Language == filter.Language);
        if (filter.Status.HasValue) query = query.Where(x => x.Status == filter.Status);
        if (filter.Level.HasValue) query = query.Where(x => x.Level == filter.Level);
        if (filter.PartOfSpeech.HasValue) query = query.Where(x => x.PartOfSpeech == filter.PartOfSpeech);
        if (!string.IsNullOrWhiteSpace(filter.Search))
        {
            var term = filter.Search.Trim();
            query = query.Where(x => EF.Functions.ILike(x.Term, $"%{term}%") ||
                (x.PersonalNotes != null && EF.Functions.ILike(x.PersonalNotes, $"%{term}%")) ||
                x.Meanings.Any(m => EF.Functions.ILike(m.Translation, $"%{term}%") || (m.Definition != null && EF.Functions.ILike(m.Definition, $"%{term}%"))));
        }
        var total = await query.CountAsync(cancellationToken);
        var entries = await query.OrderBy(x => x.Term).Skip((page - 1) * size).Take(size).ToListAsync(cancellationToken);
        return new PagedResponse<VocabularyListItemResponse> { Page = page, PageSize = size, TotalCount = total, Items = entries.Select(x => new VocabularyListItemResponse { Id = x.Id, Term = x.Term, Language = x.Language, MainTranslation = x.Meanings.OrderBy(m => m.DisplayOrder).FirstOrDefault()?.Translation ?? "", PartOfSpeech = x.PartOfSpeech, Level = x.Level, Status = x.Status }).ToList() };
    }

    public async Task<VocabularyEntryResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken) =>
        (await DetailsQuery().AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, cancellationToken)) is { } entry ? ToResponse(entry) : null;

    public async Task<VocabularyEntryResponse> CreateAsync(CreateVocabularyEntryRequest request, CancellationToken cancellationToken)
    {
        Validate(request);
        await EnsureUniqueAsync(request.Term, request.Language, null, cancellationToken);
        var entry = new VocabularyEntry(request.Term, request.Language, request.Status, request.Meanings[0].Translation);
        entry.Update(request.Term, request.Language, request.PartOfSpeech, request.Level, request.Status, request.Pronunciation, request.PersonalNotes, request.Source);
        entry.ReplaceMeanings(request.Meanings.Select(x => (x.Translation, x.Definition, x.Context)));
        entry.ReplaceExamples(request.Examples.Select(x => (x.Sentence, x.Translation, x.Explanation, x.IsGeneratedByAi)));
        SetDetails(entry, request);
        db.VocabularyEntries.Add(entry);
        await db.SaveChangesAsync(cancellationToken);
        return ToResponse(entry);
    }

    public async Task<VocabularyEntryResponse?> UpdateAsync(Guid id, UpdateVocabularyEntryRequest request, CancellationToken cancellationToken)
    {
        Validate(request);
        var entry = await DetailsQuery().FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (entry is null) return null;
        await EnsureUniqueAsync(request.Term, request.Language, id, cancellationToken);
        entry.Update(request.Term, request.Language, request.PartOfSpeech, request.Level, request.Status, request.Pronunciation, request.PersonalNotes, request.Source);
        entry.ReplaceMeanings(request.Meanings.Select(x => (x.Translation, x.Definition, x.Context)));
        entry.ReplaceExamples(request.Examples.Select(x => (x.Sentence, x.Translation, x.Explanation, x.IsGeneratedByAi)));
        entry.ClearLanguageDetails();
        SetDetails(entry, request);
        await db.SaveChangesAsync(cancellationToken);
        return ToResponse(entry);
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        var entry = await db.VocabularyEntries.FindAsync([id], cancellationToken);
        if (entry is null) return false;
        db.VocabularyEntries.Remove(entry);
        await db.SaveChangesAsync(cancellationToken);
        return true;
    }

    private IQueryable<VocabularyEntry> DetailsQuery() => db.VocabularyEntries.Include(x => x.Meanings).Include(x => x.Examples).Include(x => x.EnglishDetails).Include(x => x.GermanDetails);
    private async Task EnsureUniqueAsync(string term, Language language, Guid? ignoredId, CancellationToken ct)
    {
        var normalized = term.Trim();
        if (await db.VocabularyEntries.AnyAsync(x => x.Language == language && x.Term == normalized && (!ignoredId.HasValue || x.Id != ignoredId), ct)) throw new InvalidOperationException("Ya existe una palabra con ese término en el idioma seleccionado.");
    }
    private static void Validate(CreateVocabularyEntryRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Term)) throw new ArgumentException("El término es obligatorio.");
        if (request.Meanings.Count == 0 || request.Meanings.Any(x => string.IsNullOrWhiteSpace(x.Translation))) throw new ArgumentException("Debes registrar al menos una traducción.");
        if (request.Language != Language.English && request.EnglishDetails is not null) throw new ArgumentException("Los detalles en inglés solo corresponden a palabras en inglés.");
        if (request.Language != Language.German && request.GermanDetails is not null) throw new ArgumentException("Los detalles en alemán solo corresponden a palabras en alemán.");
    }
    private static void SetDetails(VocabularyEntry entry, CreateVocabularyEntryRequest request)
    {
        if (request.EnglishDetails is { } e) entry.SetEnglishDetails(new EnglishWordDetails(entry.Id) { PastTense = e.PastTense?.Trim(), PastParticiple = e.PastParticiple?.Trim(), ThirdPersonSingular = e.ThirdPersonSingular?.Trim(), Gerund = e.Gerund?.Trim(), IsIrregularVerb = e.IsIrregularVerb, RelatedPhrasalVerbs = e.RelatedPhrasalVerbs?.Trim() });
        if (request.GermanDetails is { } g) entry.SetGermanDetails(new GermanWordDetails(entry.Id) { Gender = g.Gender, Article = g.Article?.Trim(), Plural = g.Plural?.Trim(), AuxiliaryVerb = g.AuxiliaryVerb?.Trim(), PastParticiple = g.PastParticiple?.Trim(), IsSeparableVerb = g.IsSeparableVerb, SeparablePrefix = g.SeparablePrefix?.Trim(), GoverningCase = g.GoverningCase?.Trim() });
    }
    private static VocabularyEntryResponse ToResponse(VocabularyEntry x) => new() { Id = x.Id, Term = x.Term, Language = x.Language, PartOfSpeech = x.PartOfSpeech, Level = x.Level, Status = x.Status, Pronunciation = x.Pronunciation, PersonalNotes = x.PersonalNotes, Source = x.Source, CreatedAtUtc = x.CreatedAtUtc, UpdatedAtUtc = x.UpdatedAtUtc, Meanings = x.Meanings.OrderBy(m => m.DisplayOrder).Select(m => new VocabularyMeaningRequest { Translation = m.Translation, Definition = m.Definition, Context = m.Context }).ToList(), Examples = x.Examples.Select(e => new ExampleSentenceRequest { Sentence = e.Sentence, Translation = e.Translation, Explanation = e.Explanation, IsGeneratedByAi = e.IsGeneratedByAi }).ToList(), EnglishDetails = x.EnglishDetails is null ? null : new EnglishWordDetailsRequest { PastTense = x.EnglishDetails.PastTense, PastParticiple = x.EnglishDetails.PastParticiple, ThirdPersonSingular = x.EnglishDetails.ThirdPersonSingular, Gerund = x.EnglishDetails.Gerund, IsIrregularVerb = x.EnglishDetails.IsIrregularVerb, RelatedPhrasalVerbs = x.EnglishDetails.RelatedPhrasalVerbs }, GermanDetails = x.GermanDetails is null ? null : new GermanWordDetailsRequest { Gender = x.GermanDetails.Gender, Article = x.GermanDetails.Article, Plural = x.GermanDetails.Plural, AuxiliaryVerb = x.GermanDetails.AuxiliaryVerb, PastParticiple = x.GermanDetails.PastParticiple, IsSeparableVerb = x.GermanDetails.IsSeparableVerb, SeparablePrefix = x.GermanDetails.SeparablePrefix, GoverningCase = x.GermanDetails.GoverningCase } };
}
