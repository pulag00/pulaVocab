using pulaVocab.Domain.Enums;

namespace pulaVocab.Domain.Entities;

public class PracticeSession
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public Guid? UserId { get; private set; }
    public Guid? ParentSessionId { get; private set; }
    public Language Language { get; private set; }
    public PracticeExerciseType ExerciseType { get; private set; }
    public DateTime StartedAtUtc { get; private set; }
    public DateTime? FinishedAtUtc { get; private set; }
    public PracticeSessionStatus Status { get; private set; }
    public int ScheduledCount { get; private set; }
    public int PracticedCount { get; private set; }
    public int CorrectCount { get; private set; }
    public int IncorrectCount { get; private set; }
    public long? DurationMilliseconds { get; private set; }
    public string FiltersJson { get; private set; } = "{}";
    public ICollection<PracticeAnswer> Answers { get; private set; } = new List<PracticeAnswer>();

    private PracticeSession() { }
    public PracticeSession(Language language, PracticeExerciseType type, int count, string filtersJson, DateTime now, Guid? parentSessionId = null)
    { Language = language; ExerciseType = type; ScheduledCount = count; FiltersJson = filtersJson; StartedAtUtc = now; Status = PracticeSessionStatus.InProgress; ParentSessionId = parentSessionId; }
    public void Register(bool correct) { PracticedCount++; if (correct) CorrectCount++; else IncorrectCount++; }
    public void Finish(DateTime now, bool early) { FinishedAtUtc = now; DurationMilliseconds = (long)(now - StartedAtUtc).TotalMilliseconds; Status = early ? PracticeSessionStatus.EndedEarly : PracticeSessionStatus.Completed; }
}
