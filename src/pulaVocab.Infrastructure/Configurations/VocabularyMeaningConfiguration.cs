using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using pulaVocab.Domain.Entities;

namespace pulaVocab.Infrastructure.Configurations;

public class VocabularyMeaningConfiguration : IEntityTypeConfiguration<VocabularyMeaning>
{
    public void Configure(EntityTypeBuilder<VocabularyMeaning> builder)
    {
        builder.ToTable("vocabulary_meanings");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("id");
        builder.Property(x => x.VocabularyEntryId).HasColumnName("vocabulary_entry_id").IsRequired();
        builder.Property(x => x.Translation).HasColumnName("translation").HasMaxLength(300).IsRequired();
        builder.Property(x => x.Definition).HasColumnName("definition").HasMaxLength(1000);
        builder.Property(x => x.Context).HasColumnName("context").HasMaxLength(1000);
        builder.Property(x => x.DisplayOrder).HasColumnName("display_order").IsRequired();

        builder.HasOne(x => x.VocabularyEntry)
            .WithMany(x => x.Meanings)
            .HasForeignKey(x => x.VocabularyEntryId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
