using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using pulaVocab.Domain.Entities;

namespace pulaVocab.Infrastructure.Configurations;

public class EnglishWordDetailsConfiguration : IEntityTypeConfiguration<EnglishWordDetails>
{
    public void Configure(EntityTypeBuilder<EnglishWordDetails> builder)
    {
        builder.ToTable("english_word_details");

        builder.HasKey(x => x.VocabularyEntryId);
        builder.Property(x => x.VocabularyEntryId).HasColumnName("vocabulary_entry_id");
        builder.Property(x => x.Infinitive).HasColumnName("infinitive").HasMaxLength(200);
        builder.Property(x => x.PastTense).HasColumnName("past_tense").HasMaxLength(200);
        builder.Property(x => x.PastParticiple).HasColumnName("past_participle").HasMaxLength(200);
        builder.Property(x => x.ThirdPersonSingular).HasColumnName("third_person_singular").HasMaxLength(200);
        builder.Property(x => x.Gerund).HasColumnName("gerund").HasMaxLength(200);
        builder.Property(x => x.IsIrregularVerb).HasColumnName("is_irregular_verb").IsRequired();
        builder.Property(x => x.RelatedPhrasalVerbs).HasColumnName("related_phrasal_verbs").HasMaxLength(1000);

        builder.HasOne(x => x.VocabularyEntry)
            .WithOne(x => x.EnglishDetails)
            .HasForeignKey<EnglishWordDetails>(x => x.VocabularyEntryId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
