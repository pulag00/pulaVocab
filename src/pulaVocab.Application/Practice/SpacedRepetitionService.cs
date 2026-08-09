using pulaVocab.Domain.Entities;
using pulaVocab.Domain.Enums;

namespace pulaVocab.Application.Practice;

public sealed class SpacedRepetitionService
{
    public void Apply(WordPracticeProgress progress, PracticeRating rating, PracticeExerciseType type, DateTime now)
    {
        progress.FirstReviewedAtUtc ??= now; progress.LastReviewedAtUtc = now; progress.LastRating = rating; progress.LastExerciseType = type;
        if (rating == PracticeRating.Forgot) { progress.IncorrectCount++; progress.CorrectStreak = 0; progress.IntervalDays = 10d / 1440d; }
        else { progress.CorrectCount++; progress.CorrectStreak = rating == PracticeRating.Hard ? Math.Max(0, progress.CorrectStreak - 1) : progress.CorrectStreak + 1; progress.IntervalDays = NextInterval(progress.IntervalDays, rating); }
        progress.NextReviewAtUtc = now.AddDays(progress.IntervalDays);
        progress.MasteryLevel = IsMastered(progress) ? MasteryLevel.Mastered : rating is PracticeRating.Forgot or PracticeRating.Hard ? MasteryLevel.Difficult : MasteryLevel.Learning;
    }
    public static double NextInterval(double current, PracticeRating rating) => rating switch { PracticeRating.Forgot => 10d / 1440d, PracticeRating.Hard => current <= 0 ? 1 : Math.Max(1, current * 1.2), PracticeRating.Good => current <= 0 ? 2 : current * 2, PracticeRating.Easy => current <= 0 ? 4 : current * 2.5, _ => throw new ArgumentOutOfRangeException(nameof(rating)) };
    public static bool IsMastered(WordPracticeProgress p) => p.CorrectCount >= 5 && p.CorrectStreak >= 3 && p.IntervalDays >= 21 && p.CorrectCount / (double)Math.Max(1, p.CorrectCount + p.IncorrectCount) >= .8;
}

public static class MatchingRoundBuilder
{
    public static string Normalize(string value) => string.Join(' ', value.Split(' ', StringSplitOptions.RemoveEmptyEntries)).ToUpperInvariant();
    public static IReadOnlyList<PracticeWordResponse> RemoveAmbiguous(IEnumerable<PracticeWordResponse> words) => words.Where(x => !string.IsNullOrWhiteSpace(x.MainTranslation)).GroupBy(x => Normalize(x.MainTranslation)).Select(g => g.First()).GroupBy(x => x.Id).Select(g => g.First()).ToList();
    public static void Shuffle<T>(IList<T> items, Random random) { for (var i = items.Count - 1; i > 0; i--) { var j = random.Next(i + 1); (items[i], items[j]) = (items[j], items[i]); } }
}
