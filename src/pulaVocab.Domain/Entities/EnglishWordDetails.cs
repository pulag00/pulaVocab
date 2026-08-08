namespace pulaVocab.Domain.Entities;

public class EnglishWordDetails
{
    public Guid VocabularyEntryId { get; set; }
    public string? Infinitive { get; set; }
    public string? PastTense { get; set; }
    public string? PastParticiple { get; set; }
    public string? ThirdPersonSingular { get; set; }
    public string? Gerund { get; set; }
    public bool IsIrregularVerb { get; set; }
    public string? RelatedPhrasalVerbs { get; set; }

    public VocabularyEntry? VocabularyEntry { get; private set; }

    public EnglishWordDetails() { }

    public EnglishWordDetails(Guid vocabularyEntryId)
    {
        VocabularyEntryId = vocabularyEntryId;
    }
}
