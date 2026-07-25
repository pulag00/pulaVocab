namespace pulaVocab.Domain.Entities;

public class VocabularyMeaning
{
    public Guid Id { get; private set; }
    public Guid VocabularyEntryId { get; private set; }
    public string Translation { get; private set; }
    public string? Definition { get; private set; }
    public string? Context { get; private set; }
    public int DisplayOrder { get; private set; }

    public VocabularyEntry? VocabularyEntry { get; private set; }

    private VocabularyMeaning() { }

    public VocabularyMeaning(Guid vocabularyEntryId, string translation, string? definition, string? context, int displayOrder)
    {
        Id = Guid.NewGuid();
        VocabularyEntryId = vocabularyEntryId;
        Translation = string.IsNullOrWhiteSpace(translation) ? throw new ArgumentException("Translation is required.", nameof(translation)) : translation.Trim();
        Definition = definition;
        Context = context;
        DisplayOrder = displayOrder;
    }
}
