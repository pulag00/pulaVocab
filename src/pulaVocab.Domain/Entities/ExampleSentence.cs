namespace pulaVocab.Domain.Entities;

public class ExampleSentence
{
    public Guid Id { get; private set; }
    public Guid VocabularyEntryId { get; private set; }
    public string Sentence { get; private set; }
    public string Translation { get; private set; }
    public string? Explanation { get; private set; }
    public bool IsGeneratedByAi { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }

    public VocabularyEntry? VocabularyEntry { get; private set; }

    private ExampleSentence() { }

    public ExampleSentence(Guid vocabularyEntryId, string sentence, string translation, string? explanation, bool isGeneratedByAi, DateTime createdAtUtc)
    {
        Id = Guid.NewGuid();
        VocabularyEntryId = vocabularyEntryId;
        Sentence = string.IsNullOrWhiteSpace(sentence) ? throw new ArgumentException("Sentence is required.", nameof(sentence)) : sentence.Trim();
        Translation = string.IsNullOrWhiteSpace(translation) ? throw new ArgumentException("Translation is required.", nameof(translation)) : translation.Trim();
        Explanation = explanation;
        IsGeneratedByAi = isGeneratedByAi;
        CreatedAtUtc = createdAtUtc;
    }
}
