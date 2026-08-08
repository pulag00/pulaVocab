using pulaVocab.Domain.Enums;

namespace pulaVocab.Application.Vocabulary;

public sealed class VocabularyMeaningRequest { public string Translation { get; set; } = ""; public string? Definition { get; set; } public string? Context { get; set; } }
public sealed class ExampleSentenceRequest { public string Sentence { get; set; } = ""; public string Translation { get; set; } = ""; public string? Explanation { get; set; } public bool IsGeneratedByAi { get; set; } }
public sealed class EnglishWordDetailsRequest { public string? Infinitive { get; set; } public string? PastTense { get; set; } public string? PastParticiple { get; set; } public string? ThirdPersonSingular { get; set; } public string? Gerund { get; set; } public bool IsIrregularVerb { get; set; } public string? RelatedPhrasalVerbs { get; set; } }
public sealed class GermanWordDetailsRequest { public GermanGender? Gender { get; set; } public string? Article { get; set; } public string? Plural { get; set; } public string? AuxiliaryVerb { get; set; } public string? PastParticiple { get; set; } public bool IsSeparableVerb { get; set; } public string? SeparablePrefix { get; set; } public string? GoverningCase { get; set; } }
public class CreateVocabularyEntryRequest
{
    public string Term { get; set; } = "";
    public Language Language { get; set; }
    public PartOfSpeech? PartOfSpeech { get; set; }
    public CefrLevel? Level { get; set; }
    public LearningStatus Status { get; set; } = LearningStatus.New;
    public string? Pronunciation { get; set; }
    public string? Ipa { get; set; }
    public string? IpaAmerican { get; set; }
    public string? IpaBritish { get; set; }
    public string? Synonyms { get; set; }
    public string? Antonyms { get; set; }
    public string? RelatedTerms { get; set; }
    public string? PersonalNotes { get; set; }
    public string? Source { get; set; }
    public List<VocabularyMeaningRequest> Meanings { get; set; } = [];
    public List<ExampleSentenceRequest> Examples { get; set; } = [];
    public EnglishWordDetailsRequest? EnglishDetails { get; set; }
    public GermanWordDetailsRequest? GermanDetails { get; set; }
}
public sealed class UpdateVocabularyEntryRequest : CreateVocabularyEntryRequest { }
public sealed class VocabularyFilterRequest { public Language? Language { get; set; } public LearningStatus? Status { get; set; } public CefrLevel? Level { get; set; } public PartOfSpeech? PartOfSpeech { get; set; } public string? Search { get; set; } public int Page { get; set; } = 1; public int PageSize { get; set; } = 20; }
public sealed class VocabularyMeaningResponse { public Guid Id { get; set; } public string Translation { get; set; } = ""; public string? Definition { get; set; } public string? Context { get; set; } public int DisplayOrder { get; set; } }
public sealed class ExampleSentenceResponse { public Guid Id { get; set; } public string Sentence { get; set; } = ""; public string Translation { get; set; } = ""; public string? Explanation { get; set; } public bool IsGeneratedByAi { get; set; } }
public sealed class VocabularyEntryResponse : CreateVocabularyEntryRequest { public Guid Id { get; set; } public DateTime CreatedAtUtc { get; set; } public DateTime UpdatedAtUtc { get; set; } }
public sealed class VocabularyListItemResponse { public Guid Id { get; set; } public string Term { get; set; } = ""; public Language Language { get; set; } public string MainTranslation { get; set; } = ""; public PartOfSpeech? PartOfSpeech { get; set; } public CefrLevel? Level { get; set; } public LearningStatus Status { get; set; } }
public sealed class VocabularyLookupRequest { public string Term { get; set; } = ""; public Language Language { get; set; } public string TranslationLanguage { get; set; } = "Spanish"; }
public sealed class VocabularyLookupTranslationResponse { public string Language { get; set; } = ""; public string Text { get; set; } = ""; }
public sealed class VocabularyLookupDefinitionResponse { public string Language { get; set; } = ""; public string Text { get; set; } = ""; }
public sealed class VocabularyLookupExampleResponse { public string Sentence { get; set; } = ""; public string Translation { get; set; } = ""; }
public sealed class VocabularyLookupPhrasalVerbResponse { public string Text { get; set; } = ""; public string? Description { get; set; } }
public sealed class VocabularyLookupResponse
{
    public string Term { get; set; } = "";
    public string? NormalizedTerm { get; set; }
    public Language Language { get; set; }
    public PartOfSpeech? PartOfSpeech { get; set; }
    public CefrLevel? Level { get; set; }
    public string? Ipa { get; set; }
    public string? IpaAmerican { get; set; }
    public string? IpaBritish { get; set; }
    public List<VocabularyLookupDefinitionResponse> Definitions { get; set; } = [];
    public List<VocabularyLookupTranslationResponse> Translations { get; set; } = [];
    public List<VocabularyLookupExampleResponse> Examples { get; set; } = [];
    public List<string> Synonyms { get; set; } = [];
    public List<string> Antonyms { get; set; } = [];
    public List<string> RelatedTerms { get; set; } = [];
    public string? Notes { get; set; }
    public string? Explanation { get; set; }
    public string? Infinitive { get; set; }
    public string? Past { get; set; }
    public string? PastParticiple { get; set; }
    public string? ThirdPerson { get; set; }
    public string? Gerund { get; set; }
    public bool IsIrregular { get; set; }
    public bool IsSeparableVerb { get; set; }
    public List<VocabularyLookupPhrasalVerbResponse> PhrasalVerbs { get; set; } = [];
    public bool AlreadyExists { get; set; }
    public Guid? ExistingEntryId { get; set; }
    public string? Message { get; set; }
}
public sealed class PagedResponse<T> { public IReadOnlyList<T> Items { get; set; } = []; public int Page { get; set; } public int PageSize { get; set; } public int TotalCount { get; set; } public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize); }
