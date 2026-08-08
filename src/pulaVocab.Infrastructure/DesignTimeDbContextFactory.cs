using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace pulaVocab.Infrastructure;

public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<VocabMasterDbContext>
{
    public VocabMasterDbContext CreateDbContext(string[] args)
    {
        var basePath = Directory.GetCurrentDirectory();
        var configurationBuilder = new ConfigurationBuilder();

        var apiSettingsPath = Path.Combine(basePath, "..", "pulaVocabApi", "appsettings.json");
        if (File.Exists(apiSettingsPath))
        {
            configurationBuilder.AddJsonFile(apiSettingsPath, optional: false, reloadOnChange: false);
        }

        configurationBuilder.AddEnvironmentVariables();

        var userSecretsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Microsoft", "UserSecrets", "pulaVocab-Api-Secrets", "secrets.json");
        if (File.Exists(userSecretsPath))
        {
            configurationBuilder.AddJsonFile(userSecretsPath, optional: false, reloadOnChange: false);
        }

        var configuration = configurationBuilder.Build();
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("DefaultConnection is not configured.");

        var optionsBuilder = new DbContextOptionsBuilder<VocabMasterDbContext>();
        optionsBuilder.UseNpgsql(connectionString);

        return new VocabMasterDbContext(optionsBuilder.Options);
    }
}
