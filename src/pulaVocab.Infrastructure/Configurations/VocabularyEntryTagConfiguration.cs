using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using pulaVocab.Domain.Entities;

namespace pulaVocab.Infrastructure.Configurations;

public class VocabularyEntryTagConfiguration : IEntityTypeConfiguration<VocabularyEntryTag>
{
    public void Configure(EntityTypeBuilder<VocabularyEntryTag> builder)
    {
        builder.ToTable("vocabulary_entry_tags");

        builder.HasKey(x => new { x.VocabularyEntryId, x.VocabularyTagId });
        builder.Property(x => x.VocabularyEntryId).HasColumnName("vocabulary_entry_id");
        builder.Property(x => x.VocabularyTagId).HasColumnName("vocabulary_tag_id");

        builder.HasOne(x => x.VocabularyEntry)
            .WithMany(x => x.VocabularyEntryTags)
            .HasForeignKey(x => x.VocabularyEntryId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.VocabularyTag)
            .WithMany(x => x.VocabularyEntryTags)
            .HasForeignKey(x => x.VocabularyTagId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
