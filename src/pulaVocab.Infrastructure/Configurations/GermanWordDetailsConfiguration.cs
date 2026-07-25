using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using pulaVocab.Domain.Entities;

namespace pulaVocab.Infrastructure.Configurations;

public class GermanWordDetailsConfiguration : IEntityTypeConfiguration<GermanWordDetails>
{
    public void Configure(EntityTypeBuilder<GermanWordDetails> builder)
    {
        builder.ToTable("german_word_details");

        builder.HasKey(x => x.VocabularyEntryId);
        builder.Property(x => x.VocabularyEntryId).HasColumnName("vocabulary_entry_id");
        builder.Property(x => x.Gender).HasColumnName("gender");
        builder.Property(x => x.Article).HasColumnName("article").HasMaxLength(50);
        builder.Property(x => x.Plural).HasColumnName("plural").HasMaxLength(200);
        builder.Property(x => x.AuxiliaryVerb).HasColumnName("auxiliary_verb").HasMaxLength(50);
        builder.Property(x => x.PastParticiple).HasColumnName("past_participle").HasMaxLength(200);
        builder.Property(x => x.IsSeparableVerb).HasColumnName("is_separable_verb").IsRequired();
        builder.Property(x => x.SeparablePrefix).HasColumnName("separable_prefix").HasMaxLength(100);
        builder.Property(x => x.GoverningCase).HasColumnName("governing_case").HasMaxLength(100);

        builder.HasOne(x => x.VocabularyEntry)
            .WithOne(x => x.GermanDetails)
            .HasForeignKey<GermanWordDetails>(x => x.VocabularyEntryId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
