using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using pulaVocab.Domain.Entities;

namespace pulaVocab.Infrastructure.Configurations;

public class VocabularyTagConfiguration : IEntityTypeConfiguration<VocabularyTag>
{
    public void Configure(EntityTypeBuilder<VocabularyTag> builder)
    {
        builder.ToTable("vocabulary_tags");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("id");
        builder.Property(x => x.Name).HasColumnName("name").HasMaxLength(100).IsRequired();
        builder.Property(x => x.Color).HasColumnName("color").HasMaxLength(50);

        builder.HasMany(x => x.VocabularyEntryTags)
            .WithOne(x => x.VocabularyTag)
            .HasForeignKey(x => x.VocabularyTagId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => x.Name).IsUnique().HasDatabaseName("ix_vocabulary_tags_name");
    }
}
