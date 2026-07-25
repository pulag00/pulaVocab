using pulaVocab.Domain.Enums;

namespace pulaVocab.Domain.Entities;

public class GermanWordDetails
{
    public Guid VocabularyEntryId { get; set; }
    public GermanGender? Gender { get; set; }
    public string? Article { get; set; }
    public string? Plural { get; set; }
    public string? AuxiliaryVerb { get; set; }
    public string? PastParticiple { get; set; }
    public bool IsSeparableVerb { get; set; }
    public string? SeparablePrefix { get; set; }
    public string? GoverningCase { get; set; }

    public VocabularyEntry? VocabularyEntry { get; private set; }

    public GermanWordDetails() { }

    public GermanWordDetails(Guid vocabularyEntryId)
    {
        VocabularyEntryId = vocabularyEntryId;
    }
}
