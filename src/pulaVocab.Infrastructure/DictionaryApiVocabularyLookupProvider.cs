using System.Net.Http.Json;
using Microsoft.Extensions.Logging;
using pulaVocab.Application.Vocabulary;

namespace pulaVocab.Infrastructure;

public sealed class DictionaryApiVocabularyLookupProvider : IVocabularyLookupProvider
{
    private readonly HttpClient httpClient;
    private readonly ILogger<DictionaryApiVocabularyLookupProvider> logger;

    public DictionaryApiVocabularyLookupProvider(HttpClient httpClient, ILogger<DictionaryApiVocabularyLookupProvider> logger)
    {
        this.httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<VocabularyLookupResponse> LookupAsync(VocabularyLookupRequest request, CancellationToken cancellationToken = default)
    {
        if (request is null) throw new ArgumentNullException(nameof(request));
        var term = request.Term?.Trim() ?? string.Empty;
        if (term.Length == 0) throw new ArgumentException("El término es obligatorio.", nameof(request));

        var lang = request.Language == Domain.Enums.Language.German ? "de" : "en";
        var url = $"https://api.dictionaryapi.dev/api/v2/entries/{lang}/{Uri.EscapeDataString(term)}";

        HttpResponseMessage resp;
        try
        {
            resp = await httpClient.GetAsync(url, cancellationToken).ConfigureAwait(false);
        }
        catch (HttpRequestException ex)
        {
            logger.LogWarning(ex, "DictionaryApi request failed for {Term}", term);
            throw new InvalidOperationException("No se pudo contactar al proveedor de diccionario.", ex);
        }

        if (!resp.IsSuccessStatusCode)
        {
            var text = await resp.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
            logger.LogWarning("DictionaryApi returned {Status} for {Term}: {Text}", (int)resp.StatusCode, term, text);
            throw new InvalidOperationException($"Proveedor de diccionario respondió {(int)resp.StatusCode}.");
        }

        var json = await resp.Content.ReadFromJsonAsync<object>(cancellationToken: cancellationToken).ConfigureAwait(false);

        // Best-effort mapping: the API returns a list; map basic fields
        var result = new VocabularyLookupResponse
        {
            Term = term,
            NormalizedTerm = term.ToLowerInvariant(),
            Language = request.Language,
            Definitions = new List<VocabularyLookupDefinitionResponse>(),
            Translations = new List<VocabularyLookupTranslationResponse>(),
            Examples = new List<VocabularyLookupExampleResponse>(),
            Synonyms = new List<string>(),
            Antonyms = new List<string>(),
            RelatedTerms = new List<string>(),
            PhrasalVerbs = new List<VocabularyLookupPhrasalVerbResponse>()
        };

        try
        {
            using var doc = System.Text.Json.JsonDocument.Parse(System.Text.Json.JsonSerializer.Serialize(json));
            if (doc.RootElement.ValueKind == System.Text.Json.JsonValueKind.Array && doc.RootElement.GetArrayLength() > 0)
            {
                var first = doc.RootElement[0];

                if (first.TryGetProperty("word", out var word)) result.Term = word.GetString() ?? result.Term;

                if (first.TryGetProperty("phonetics", out var phonetics) && phonetics.ValueKind == System.Text.Json.JsonValueKind.Array)
                {
                    foreach (var p in phonetics.EnumerateArray())
                    {
                        if (p.TryGetProperty("text", out var t) && !string.IsNullOrWhiteSpace(t.GetString()))
                        {
                            result.Ipa = t.GetString();
                            break;
                        }
                    }
                }

                if (first.TryGetProperty("meanings", out var meanings) && meanings.ValueKind == System.Text.Json.JsonValueKind.Array)
                {
                    foreach (var meaning in meanings.EnumerateArray())
                    {
                        var part = meaning.TryGetProperty("partOfSpeech", out var ps) ? ps.GetString() : null;
                        if (meaning.TryGetProperty("definitions", out var defs) && defs.ValueKind == System.Text.Json.JsonValueKind.Array)
                        {
                            foreach (var d in defs.EnumerateArray())
                            {
                                if (d.TryGetProperty("definition", out var def))
                                {
                                    result.Definitions.Add(new VocabularyLookupDefinitionResponse { Language = request.Language.ToString(), Text = def.GetString() ?? string.Empty });
                                }
                                if (d.TryGetProperty("example", out var ex) && !string.IsNullOrWhiteSpace(ex.GetString()))
                                {
                                    result.Examples.Add(new VocabularyLookupExampleResponse { Sentence = ex.GetString() ?? string.Empty, Translation = string.Empty });
                                }
                            }
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Failed to parse dictionaryapi.dev response for {Term}", term);
        }

        return result;
    }
}
