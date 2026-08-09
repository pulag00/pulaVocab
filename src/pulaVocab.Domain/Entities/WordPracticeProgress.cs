using pulaVocab.Domain.Enums;

namespace pulaVocab.Domain.Entities;

public class WordPracticeProgress
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public Guid? UserId { get; private set; }
    public Guid VocabularyEntryId { get; private set; }
    public DateTime? FirstReviewedAtUtc { get; set; }
    public DateTime? LastReviewedAtUtc { get; set; }
    public DateTime? NextReviewAtUtc { get; set; }
    public double IntervalDays { get; set; }
    public int CorrectCount { get; set; }
    public int IncorrectCount { get; set; }
    public int CorrectStreak { get; set; }
    public MasteryLevel MasteryLevel { get; set; }
    public PracticeRating? LastRating { get; set; }
    public PracticeExerciseType? LastExerciseType { get; set; }
    public VocabularyEntry? VocabularyEntry { get; private set; }
    private WordPracticeProgress() { }
    public WordPracticeProgress(Guid vocabularyEntryId) { VocabularyEntryId = vocabularyEntryId; MasteryLevel = MasteryLevel.New; }
}
