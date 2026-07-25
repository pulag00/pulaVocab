using pulaVocab.Domain.Entities;
using pulaVocab.Domain.Enums;
using Xunit;

namespace pulaVocab.Tests;

public class VocabularyDomainTests
{
    [Fact]
    public void EnglishWordCanHaveEnglishDetails()
    {
        var entry = new VocabularyEntry("book", Language.English, LearningStatus.New, "libro");

        entry.SetEnglishDetails(new EnglishWordDetails
        {
            PastTense = "booked",
            IsIrregularVerb = false
        });

        Assert.NotNull(entry.EnglishDetails);
        Assert.Equal("booked", entry.EnglishDetails!.PastTense);
    }

    [Fact]
    public void GermanWordCanHaveGermanDetails()
    {
        var entry = new VocabularyEntry("Haus", Language.German, LearningStatus.New, "house");

        entry.SetGermanDetails(new GermanWordDetails
        {
            Gender = GermanGender.Neuter,
            Article = "das",
            Plural = "Häuser"
        });

        Assert.NotNull(entry.GermanDetails);
        Assert.Equal(GermanGender.Neuter, entry.GermanDetails!.Gender);
    }

    [Fact]
    public void GermanEntryCannotReceiveEnglishDetails()
    {
        var entry = new VocabularyEntry("Haus", Language.German, LearningStatus.New, "house");

        Assert.Throws<InvalidOperationException>(() => entry.SetEnglishDetails(new EnglishWordDetails()));
    }

    [Fact]
    public void EnglishEntryCannotReceiveGermanDetails()
    {
        var entry = new VocabularyEntry("book", Language.English, LearningStatus.New, "libro");

        Assert.Throws<InvalidOperationException>(() => entry.SetGermanDetails(new GermanWordDetails()));
    }

    [Fact]
    public void TermCannotBeEmpty()
    {
        Assert.Throws<ArgumentException>(() => new VocabularyEntry("   ", Language.English, LearningStatus.New, "libro"));
    }

    [Fact]
    public void TranslationCannotBeEmpty()
    {
        Assert.Throws<ArgumentException>(() => new VocabularyEntry("book", Language.English, LearningStatus.New, "   "));
    }
}
