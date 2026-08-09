using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using pulaVocab.Infrastructure;
using pulaVocab.Application.Vocabulary;
using pulaVocab.Application.Practice;

namespace pulaVocab.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        services.AddDbContext<VocabMasterDbContext>(options =>
            options.UseNpgsql(connectionString));
        services.AddScoped<IVocabularyService, VocabularyService>();
        services.AddScoped<IVocabularyLookupService, VocabularyLookupService>();
        services.AddScoped<IPracticeService, PracticeService>();
        services.AddSingleton<SpacedRepetitionService>();

        var providerEndpoint = configuration.GetSection("VocabularyLookup").GetValue<string>("ProviderEndpoint");
        if (!string.IsNullOrWhiteSpace(providerEndpoint))
        {
            // Support named built-in provider shortcuts
            if (string.Equals(providerEndpoint, "dictionaryapi", StringComparison.OrdinalIgnoreCase))
            {
                services.AddHttpClient<IVocabularyLookupProvider, DictionaryApiVocabularyLookupProvider>();
            }
            else if (string.Equals(providerEndpoint, "datamuse+translate", StringComparison.OrdinalIgnoreCase) || string.Equals(providerEndpoint, "datamuse-translate", StringComparison.OrdinalIgnoreCase))
            {
                services.AddHttpClient<IVocabularyLookupProvider, DatamuseTranslateVocabularyLookupProvider>();
            }
            else if (providerEndpoint.StartsWith("http", StringComparison.OrdinalIgnoreCase))
            {
                services.AddHttpClient<IVocabularyLookupProvider, HttpVocabularyLookupProvider>();
                services.Configure<VocabularyLookupOptions>(configuration.GetSection("VocabularyLookup"));
            }
            else
            {
                // Unknown value — fall back to local provider
                services.AddScoped<IVocabularyLookupProvider, LocalVocabularyLookupProvider>();
            }
        }
        else
        {
            services.AddScoped<IVocabularyLookupProvider, LocalVocabularyLookupProvider>();
        }

        return services;
    }
}
