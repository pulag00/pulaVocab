using Microsoft.EntityFrameworkCore;
using pulaVocab.Domain.Entities;

namespace pulaVocab.Infrastructure;

public class VocabMasterDbContext : DbContext
{
    public VocabMasterDbContext(DbContextOptions<VocabMasterDbContext> options) : base(options)
    {
    }

    public DbSet<VocabularyEntry> VocabularyEntries => Set<VocabularyEntry>();
    public DbSet<VocabularyMeaning> VocabularyMeanings => Set<VocabularyMeaning>();
    public DbSet<ExampleSentence> ExampleSentences => Set<ExampleSentence>();
    public DbSet<EnglishWordDetails> EnglishWordDetails => Set<EnglishWordDetails>();
    public DbSet<GermanWordDetails> GermanWordDetails => Set<GermanWordDetails>();
    public DbSet<VocabularyTag> VocabularyTags => Set<VocabularyTag>();
    public DbSet<VocabularyEntryTag> VocabularyEntryTags => Set<VocabularyEntryTag>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(VocabMasterDbContext).Assembly);
    }
}
