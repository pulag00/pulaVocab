using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using pulaVocab.Infrastructure;
using pulaVocab.Application.Vocabulary;

namespace pulaVocab.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        services.AddDbContext<VocabMasterDbContext>(options =>
            options.UseNpgsql(connectionString));
        services.AddScoped<IVocabularyService, VocabularyService>();

        return services;
    }
}
