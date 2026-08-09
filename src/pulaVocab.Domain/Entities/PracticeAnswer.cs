using pulaVocab.Domain.Enums;

namespace pulaVocab.Domain.Entities;

public class PracticeAnswer
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public Guid PracticeSessionId { get; private set; }
    public Guid VocabularyEntryId { get; private set; }
    public PracticeExerciseType ExerciseType { get; private set; }
    public string Answer { get; private set; } = "";
    public bool IsCorrect { get; private set; }
    public DateTime AnsweredAtUtc { get; private set; }
    public int AttemptNumber { get; private set; }
    public long? ResponseTimeMilliseconds { get; private set; }
    public PracticeSession? PracticeSession { get; private set; }
    public VocabularyEntry? VocabularyEntry { get; private set; }
    private PracticeAnswer() { }
    public PracticeAnswer(Guid sessionId, Guid wordId, PracticeExerciseType type, string answer, bool correct, int attempt, long? elapsed, DateTime now)
    { PracticeSessionId = sessionId; VocabularyEntryId = wordId; ExerciseType = type; Answer = answer; IsCorrect = correct; AttemptNumber = attempt; ResponseTimeMilliseconds = elapsed; AnsweredAtUtc = now; }
}
