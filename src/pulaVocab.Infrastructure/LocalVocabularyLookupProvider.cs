using System.Threading;
using System.Threading.Tasks;
using pulaVocab.Application.Vocabulary;

namespace pulaVocab.Infrastructure;

public sealed class LocalVocabularyLookupProvider : IVocabularyLookupProvider
{
    public Task<VocabularyLookupResponse> LookupAsync(VocabularyLookupRequest request, CancellationToken cancellationToken = default)
    {
        var term = request.Term?.Trim() ?? string.Empty;

        var response = new VocabularyLookupResponse
        {
            Term = term,
            NormalizedTerm = term.ToLowerInvariant(),
            Language = request.Language,
            Definitions = new List<VocabularyLookupDefinitionResponse>
            {
                new() { Language = request.Language.ToString(), Text = $"Definición de {term}." }
            },
            Translations = new List<VocabularyLookupTranslationResponse>
            {
                new() { Language = request.TranslationLanguage, Text = $"Traducción de {term}." }
            },
            Examples = new List<VocabularyLookupExampleResponse>
            {
                new() { Sentence = $"Example sentence for {term}.", Translation = $"Ejemplo para {term}." }
            }
        };

        return Task.FromResult(response);
    }
}
