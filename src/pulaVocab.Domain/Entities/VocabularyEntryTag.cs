namespace pulaVocab.Domain.Entities;

public class VocabularyEntryTag
{
    public Guid VocabularyEntryId { get; private set; }
    public Guid VocabularyTagId { get; private set; }

    public VocabularyEntry? VocabularyEntry { get; private set; }
    public VocabularyTag? VocabularyTag { get; private set; }

    private VocabularyEntryTag() { }

    public VocabularyEntryTag(Guid vocabularyEntryId, Guid vocabularyTagId)
    {
        VocabularyEntryId = vocabularyEntryId;
        VocabularyTagId = vocabularyTagId;
    }
}
