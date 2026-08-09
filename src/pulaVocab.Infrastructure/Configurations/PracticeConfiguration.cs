using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using pulaVocab.Domain.Entities;

namespace pulaVocab.Infrastructure.Configurations;

public sealed class PracticeSessionConfiguration : IEntityTypeConfiguration<PracticeSession>
{
    public void Configure(EntityTypeBuilder<PracticeSession> b) { b.ToTable("practice_sessions"); b.HasKey(x => x.Id); b.Property(x => x.FiltersJson).HasColumnType("jsonb"); b.HasIndex(x => new { x.UserId, x.StartedAtUtc }); b.HasIndex(x => x.Status); b.HasMany(x => x.Answers).WithOne(x => x.PracticeSession).HasForeignKey(x => x.PracticeSessionId).OnDelete(DeleteBehavior.Cascade); }
}
public sealed class PracticeAnswerConfiguration : IEntityTypeConfiguration<PracticeAnswer>
{
    public void Configure(EntityTypeBuilder<PracticeAnswer> b) { b.ToTable("practice_answers"); b.HasKey(x => x.Id); b.Property(x => x.Answer).HasMaxLength(500); b.HasIndex(x => new { x.PracticeSessionId, x.VocabularyEntryId, x.AttemptNumber }).IsUnique(); b.HasOne(x => x.VocabularyEntry).WithMany(x => x.PracticeAnswers).HasForeignKey(x => x.VocabularyEntryId).OnDelete(DeleteBehavior.Restrict); }
}
public sealed class WordPracticeProgressConfiguration : IEntityTypeConfiguration<WordPracticeProgress>
{
    public void Configure(EntityTypeBuilder<WordPracticeProgress> b) { b.ToTable("word_practice_progress"); b.HasKey(x => x.Id); b.HasIndex(x => new { x.UserId, x.VocabularyEntryId }).IsUnique(); b.HasIndex(x => x.NextReviewAtUtc); b.HasOne(x => x.VocabularyEntry).WithOne(x => x.PracticeProgress).HasForeignKey<WordPracticeProgress>(x => x.VocabularyEntryId).OnDelete(DeleteBehavior.Cascade); }
}
