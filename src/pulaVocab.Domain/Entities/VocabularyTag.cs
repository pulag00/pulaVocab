namespace pulaVocab.Domain.Entities;

public class VocabularyTag
{
    public Guid Id { get; private set; }
    public string Name { get; private set; }
    public string? Color { get; private set; }

    public ICollection<VocabularyEntryTag> VocabularyEntryTags { get; private set; } = new List<VocabularyEntryTag>();

    private VocabularyTag() { }

    public VocabularyTag(string name, string? color)
    {
        Id = Guid.NewGuid();
        Name = string.IsNullOrWhiteSpace(name) ? throw new ArgumentException("Tag name is required.", nameof(name)) : name.Trim();
        Color = color;
    }
}
