using pulaVocab.Domain.Enums;

namespace pulaVocab.Domain.Entities;

public class VocabularyEntry
{
    public Guid Id { get; private set; }
    public string Term { get; private set; }
    public Language Language { get; private set; }
    public PartOfSpeech? PartOfSpeech { get; private set; }
    public CefrLevel? Level { get; private set; }
    public LearningStatus Status { get; private set; }
    public string? Pronunciation { get; private set; }
    public string? PersonalNotes { get; private set; }
    public string? Source { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }
    public DateTime UpdatedAtUtc { get; private set; }

    public ICollection<VocabularyMeaning> Meanings { get; private set; } = new List<VocabularyMeaning>();
    public ICollection<ExampleSentence> Examples { get; private set; } = new List<ExampleSentence>();
    public EnglishWordDetails? EnglishDetails { get; private set; }
    public GermanWordDetails? GermanDetails { get; private set; }
    public ICollection<VocabularyEntryTag> VocabularyEntryTags { get; private set; } = new List<VocabularyEntryTag>();

    private VocabularyEntry() { }

    public VocabularyEntry(string term, Language language, LearningStatus status, string translation)
    {
        Id = Guid.NewGuid();
        Term = string.IsNullOrWhiteSpace(term) ? throw new ArgumentException("Term is required.", nameof(term)) : term.Trim();
        Language = language;
        Status = status;
        Pronunciation = null;
        PersonalNotes = null;
        Source = null;
        CreatedAtUtc = DateTime.UtcNow;
        UpdatedAtUtc = CreatedAtUtc;

        AddMeaning(translation);
    }

    public void SetPartOfSpeech(PartOfSpeech partOfSpeech) => PartOfSpeech = partOfSpeech;
    public void SetLevel(CefrLevel level) => Level = level;
    public void SetPronunciation(string? pronunciation) => Pronunciation = pronunciation;
    public void SetPersonalNotes(string? personalNotes) => PersonalNotes = personalNotes;
    public void SetSource(string? source) => Source = source;
    public void SetStatus(LearningStatus status) => Status = status;

    public void Update(string term, Language language, PartOfSpeech? partOfSpeech, CefrLevel? level, LearningStatus status,
        string? pronunciation, string? personalNotes, string? source)
    {
        Term = string.IsNullOrWhiteSpace(term) ? throw new ArgumentException("Term is required.", nameof(term)) : term.Trim();
        Language = language;
        PartOfSpeech = partOfSpeech;
        Level = level;
        Status = status;
        Pronunciation = Normalize(pronunciation);
        PersonalNotes = Normalize(personalNotes);
        Source = Normalize(source);
        UpdatedAtUtc = DateTime.UtcNow;
    }

    public void ReplaceMeanings(IEnumerable<(string Translation, string? Definition, string? Context)> meanings)
    {
        var normalized = meanings.ToList();
        if (normalized.Count == 0) throw new ArgumentException("At least one translation is required.", nameof(meanings));
        Meanings.Clear();
        foreach (var meaning in normalized) AddMeaning(meaning.Translation, Normalize(meaning.Definition), Normalize(meaning.Context));
    }

    public void ReplaceExamples(IEnumerable<(string Sentence, string Translation, string? Explanation, bool IsGeneratedByAi)> examples)
    {
        Examples.Clear();
        foreach (var example in examples) AddExample(example.Sentence, example.Translation, Normalize(example.Explanation), example.IsGeneratedByAi);
    }

    public void ClearLanguageDetails()
    {
        EnglishDetails = null;
        GermanDetails = null;
    }

    private static string? Normalize(string? value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim();

    public VocabularyMeaning AddMeaning(string translation, string? definition = null, string? context = null)
    {
        if (string.IsNullOrWhiteSpace(translation))
        {
            throw new ArgumentException("Translation is required.", nameof(translation));
        }

        var meaning = new VocabularyMeaning(Id, translation, definition, context, Meanings.Count);
        Meanings.Add(meaning);
        UpdatedAtUtc = DateTime.UtcNow;
        return meaning;
    }

    public void AddExample(string sentence, string translation, string? explanation = null, bool isGeneratedByAi = false)
    {
        Examples.Add(new ExampleSentence(Id, sentence, translation, explanation, isGeneratedByAi, DateTime.UtcNow));
        UpdatedAtUtc = DateTime.UtcNow;
    }

    public void SetEnglishDetails(EnglishWordDetails details)
    {
        if (Language != Language.English)
        {
            throw new InvalidOperationException("EnglishWordDetails can only be used for English entries.");
        }

        EnglishDetails = details;
        UpdatedAtUtc = DateTime.UtcNow;
        ValidateState();
    }

    public void SetGermanDetails(GermanWordDetails details)
    {
        if (Language != Language.German)
        {
            throw new InvalidOperationException("GermanWordDetails can only be used for German entries.");
        }

        GermanDetails = details;
        UpdatedAtUtc = DateTime.UtcNow;
        ValidateState();
    }

    public void ValidateState()
    {
        if (string.IsNullOrWhiteSpace(Term))
        {
            throw new InvalidOperationException("Term cannot be empty.");
        }

        if (Meanings.Count == 0)
        {
            throw new InvalidOperationException("At least one translation is required.");
        }
    }
}
