using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using pulaVocab.Domain.Entities;

namespace pulaVocab.Infrastructure.Configurations;

public class VocabularyEntryConfiguration : IEntityTypeConfiguration<VocabularyEntry>
{
    public void Configure(EntityTypeBuilder<VocabularyEntry> builder)
    {
        builder.ToTable("vocabulary_entries");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("id");
        builder.Property(x => x.Term).HasColumnName("term").HasMaxLength(200).IsRequired();
        builder.Property(x => x.NormalizedTerm).HasColumnName("normalized_term").HasMaxLength(200).IsRequired();
        builder.Property(x => x.Language).HasColumnName("language").IsRequired();
        builder.Property(x => x.PartOfSpeech).HasColumnName("part_of_speech");
        builder.Property(x => x.Level).HasColumnName("cefr_level");
        builder.Property(x => x.Status).HasColumnName("status").IsRequired();
        builder.Property(x => x.Pronunciation).HasColumnName("pronunciation").HasMaxLength(200);
        builder.Property(x => x.Ipa).HasColumnName("ipa").HasMaxLength(200);
        builder.Property(x => x.IpaAmerican).HasColumnName("ipa_american").HasMaxLength(200);
        builder.Property(x => x.IpaBritish).HasColumnName("ipa_british").HasMaxLength(200);
        builder.Property(x => x.Synonyms).HasColumnName("synonyms").HasMaxLength(1000);
        builder.Property(x => x.Antonyms).HasColumnName("antonyms").HasMaxLength(1000);
        builder.Property(x => x.RelatedTerms).HasColumnName("related_terms").HasMaxLength(1000);
        builder.Property(x => x.PersonalNotes).HasColumnName("personal_notes").HasMaxLength(2000);
        builder.Property(x => x.Source).HasColumnName("source").HasMaxLength(500);
        builder.Property(x => x.CreatedAtUtc).HasColumnName("created_at_utc").IsRequired();
        builder.Property(x => x.UpdatedAtUtc).HasColumnName("updated_at_utc").IsRequired();

        builder.HasMany(x => x.Meanings)
            .WithOne(x => x.VocabularyEntry)
            .HasForeignKey(x => x.VocabularyEntryId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(x => x.Examples)
            .WithOne(x => x.VocabularyEntry)
            .HasForeignKey(x => x.VocabularyEntryId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.EnglishDetails)
            .WithOne(x => x.VocabularyEntry)
            .HasForeignKey<EnglishWordDetails>(x => x.VocabularyEntryId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.GermanDetails)
            .WithOne(x => x.VocabularyEntry)
            .HasForeignKey<GermanWordDetails>(x => x.VocabularyEntryId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(x => x.VocabularyEntryTags)
            .WithOne(x => x.VocabularyEntry)
            .HasForeignKey(x => x.VocabularyEntryId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => x.Language).HasDatabaseName("ix_vocabulary_entries_language");
        builder.HasIndex(x => x.Status).HasDatabaseName("ix_vocabulary_entries_status");
        builder.HasIndex(x => x.Term).HasDatabaseName("ix_vocabulary_entries_term");
        builder.HasIndex(x => new { x.Language, x.NormalizedTerm }).IsUnique().HasDatabaseName("ix_vocabulary_entries_language_normalized_term");
    }
}
