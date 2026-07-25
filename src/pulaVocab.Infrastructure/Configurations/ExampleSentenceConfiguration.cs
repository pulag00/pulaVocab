using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using pulaVocab.Domain.Entities;

namespace pulaVocab.Infrastructure.Configurations;

public class ExampleSentenceConfiguration : IEntityTypeConfiguration<ExampleSentence>
{
    public void Configure(EntityTypeBuilder<ExampleSentence> builder)
    {
        builder.ToTable("example_sentences");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("id");
        builder.Property(x => x.VocabularyEntryId).HasColumnName("vocabulary_entry_id").IsRequired();
        builder.Property(x => x.Sentence).HasColumnName("sentence").HasMaxLength(1000).IsRequired();
        builder.Property(x => x.Translation).HasColumnName("translation").HasMaxLength(1000).IsRequired();
        builder.Property(x => x.Explanation).HasColumnName("explanation").HasMaxLength(2000);
        builder.Property(x => x.IsGeneratedByAi).HasColumnName("is_generated_by_ai").IsRequired();
        builder.Property(x => x.CreatedAtUtc).HasColumnName("created_at_utc").IsRequired();

        builder.HasOne(x => x.VocabularyEntry)
            .WithMany(x => x.Examples)
            .HasForeignKey(x => x.VocabularyEntryId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
