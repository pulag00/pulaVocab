using pulaVocab.Application.Practice;
using pulaVocab.Domain.Entities;
using pulaVocab.Domain.Enums;
namespace pulaVocab.Tests;
public class PracticeTests
{
 static readonly DateTime Now=new(2026,8,9,12,0,0,DateTimeKind.Utc);
 [Fact] public void ForgotResetsStreakAndSchedulesTenMinutes(){var p=P(3,2,5);new SpacedRepetitionService().Apply(p,PracticeRating.Forgot,PracticeExerciseType.Flashcards,Now);Assert.Equal(0,p.CorrectStreak);Assert.Equal(Now.AddMinutes(10),p.NextReviewAtUtc!.Value,TimeSpan.FromSeconds(1));Assert.Equal(3,p.IncorrectCount);}
 [Fact] public void HardStartsAtOneDay(){var p=P();new SpacedRepetitionService().Apply(p,PracticeRating.Hard,PracticeExerciseType.Flashcards,Now);Assert.Equal(1,p.IntervalDays);}
 [Fact] public void HardMultipliesExistingInterval()=>Assert.Equal(12,SpacedRepetitionService.NextInterval(10,PracticeRating.Hard));
 [Fact] public void GoodStartsAtTwoDaysAndIncrementsStreak(){var p=P();new SpacedRepetitionService().Apply(p,PracticeRating.Good,PracticeExerciseType.Flashcards,Now);Assert.Equal(2,p.IntervalDays);Assert.Equal(1,p.CorrectStreak);Assert.Equal(Now.AddDays(2),p.NextReviewAtUtc);}
 [Fact] public void EasyStartsAtFourDays()=>Assert.Equal(4,SpacedRepetitionService.NextInterval(0,PracticeRating.Easy));
 [Fact] public void IntervalsGrow(){Assert.Equal(20,SpacedRepetitionService.NextInterval(10,PracticeRating.Good));Assert.Equal(25,SpacedRepetitionService.NextInterval(10,PracticeRating.Easy));}
 [Fact] public void MasteryRequiresEveryThreshold(){var p=P(5,1,3);p.IntervalDays=21;Assert.True(SpacedRepetitionService.IsMastered(p));p.CorrectCount=4;Assert.False(SpacedRepetitionService.IsMastered(p));}
 [Fact] public void MatchingCorrectIncreasesProgress(){var p=P();new SpacedRepetitionService().Apply(p,PracticeRating.Good,PracticeExerciseType.Matching,Now);Assert.Equal(1,p.CorrectCount);}
 [Fact] public void MatchingErrorSurvivesLaterCorrection(){var p=P();var s=new SpacedRepetitionService();s.Apply(p,PracticeRating.Forgot,PracticeExerciseType.Matching,Now);s.Apply(p,PracticeRating.Good,PracticeExerciseType.Matching,Now.AddMinutes(1));Assert.Equal(1,p.IncorrectCount);Assert.Equal(1,p.CorrectCount);}
 [Fact] public void AmbiguousTranslationsAreRemovedWithoutMutation(){var a=W("one","  Uno "),b=W("first","uno"),c=W("two","dos");var r=MatchingRoundBuilder.RemoveAmbiguous([a,b,c]);Assert.Equal(2,r.Count);Assert.Equal("  Uno ",a.MainTranslation);}
 [Fact] public void DuplicateWordsAreRemoved(){var a=W("one","uno");Assert.Single(MatchingRoundBuilder.RemoveAmbiguous([a,a]));}
 [Fact] public void NormalizationIgnoresCaseAndSpaces()=>Assert.Equal(MatchingRoundBuilder.Normalize(" el   libro "),MatchingRoundBuilder.Normalize("EL LIBRO"));
 [Fact] public void FisherYatesPreservesItems(){var x=Enumerable.Range(1,10).ToList();MatchingRoundBuilder.Shuffle(x,new Random(7));Assert.Equal(Enumerable.Range(1,10),x.Order());Assert.NotEqual(Enumerable.Range(1,10),x);}
 [Fact] public void EarlyFinishCountsOnlyAnswers(){var s=new PracticeSession(Language.English,PracticeExerciseType.Flashcards,10,"{}",Now);s.Register(false);s.Finish(Now.AddMinutes(1),true);Assert.Equal(PracticeSessionStatus.EndedEarly,s.Status);Assert.Equal(1,s.PracticedCount);Assert.Equal(1,s.IncorrectCount);}
 [Fact] public void NeverShownWordsAreNotErrors(){var s=new PracticeSession(Language.German,PracticeExerciseType.Flashcards,10,"{}",Now);s.Finish(Now,true);Assert.Equal(0,s.IncorrectCount);}
 [Theory][InlineData(Language.English)][InlineData(Language.German)]public void FilterHasOneLanguage(Language l)=>Assert.Equal(l,new PracticeFilterRequest{Language=l}.Language);
 [Fact] public void SessionHasExactlyOneLanguage()=>Assert.Equal(Language.German,new PracticeSession(Language.German,PracticeExerciseType.Matching,4,"{}",Now).Language);
 [Fact] public void NullComplementaryDataIsSupported(){var w=new PracticeWordResponse{Term="word",MainTranslation="palabra"};Assert.Null(w.EnglishDetails);Assert.Empty(w.Examples);}
 [Fact] public void DueWordsHavePriority(){var x=new[]{(Id:1,Due:(DateTime?)Now.AddDays(1),Hard:false),(Id:2,Due:(DateTime?)Now.AddDays(-1),Hard:false),(Id:3,Due:(DateTime?)null,Hard:true)};Assert.Equal(new[]{2,3,1},x.OrderBy(v=>v.Due<=Now?0:v.Hard?1:v.Due is null?2:3).Select(v=>v.Id));}
 static WordPracticeProgress P(int c=0,int i=0,int s=0)=>new(Guid.NewGuid()){CorrectCount=c,IncorrectCount=i,CorrectStreak=s}; static PracticeWordResponse W(string t,string m)=>new(){Id=Guid.NewGuid(),Term=t,MainTranslation=m};
}
