using System.Net.Http.Json;
using Microsoft.Extensions.Logging;
using pulaVocab.Application.Vocabulary;

namespace pulaVocab.Infrastructure;

public sealed class DatamuseTranslateVocabularyLookupProvider : IVocabularyLookupProvider
{
    private readonly HttpClient httpClient;
    private readonly ILogger<DatamuseTranslateVocabularyLookupProvider> logger;

    public DatamuseTranslateVocabularyLookupProvider(HttpClient httpClient, ILogger<DatamuseTranslateVocabularyLookupProvider> logger)
    {
        this.httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<VocabularyLookupResponse> LookupAsync(VocabularyLookupRequest request, CancellationToken cancellationToken = default)
    {
        if (request is null) throw new ArgumentNullException(nameof(request));
        var term = request.Term?.Trim() ?? string.Empty;
        if (term.Length == 0) throw new ArgumentException("El término es obligatorio.", nameof(request));

        var response = new VocabularyLookupResponse
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
            // Definitions and examples from dictionaryapi.dev
            var dictLang = request.Language == Domain.Enums.Language.German ? "de" : "en";
            var dictUrl = $"https://api.dictionaryapi.dev/api/v2/entries/{dictLang}/{Uri.EscapeDataString(term)}";
            try
            {
                var dictResp = await httpClient.GetAsync(dictUrl, cancellationToken).ConfigureAwait(false);
                if (dictResp.IsSuccessStatusCode)
                {
                    var json = await dictResp.Content.ReadFromJsonAsync<object>(cancellationToken: cancellationToken).ConfigureAwait(false);
                    using var doc = System.Text.Json.JsonDocument.Parse(System.Text.Json.JsonSerializer.Serialize(json));
                    if (doc.RootElement.ValueKind == System.Text.Json.JsonValueKind.Array && doc.RootElement.GetArrayLength() > 0)
                    {
                        var first = doc.RootElement[0];
                        if (first.TryGetProperty("phonetics", out var phonetics) && phonetics.ValueKind == System.Text.Json.JsonValueKind.Array)
                        {
                            foreach (var p in phonetics.EnumerateArray())
                            {
                                if (p.TryGetProperty("text", out var t) && !string.IsNullOrWhiteSpace(t.GetString()))
                                {
                                    response.Ipa = t.GetString();
                                    break;
                                }
                            }
                        }
                        if (first.TryGetProperty("meanings", out var meanings) && meanings.ValueKind == System.Text.Json.JsonValueKind.Array)
                        {
                            foreach (var meaning in meanings.EnumerateArray())
                            {
                                if (meaning.TryGetProperty("definitions", out var defs) && defs.ValueKind == System.Text.Json.JsonValueKind.Array)
                                {
                                    foreach (var d in defs.EnumerateArray())
                                    {
                                        if (d.TryGetProperty("definition", out var def))
                                        {
                                            response.Definitions.Add(new VocabularyLookupDefinitionResponse { Language = request.Language.ToString(), Text = def.GetString() ?? string.Empty });
                                        }
                                        if (d.TryGetProperty("example", out var ex) && !string.IsNullOrWhiteSpace(ex.GetString()))
                                        {
                                            response.Examples.Add(new VocabularyLookupExampleResponse { Sentence = ex.GetString() ?? string.Empty, Translation = string.Empty });
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogDebug(ex, "dictionaryapi.dev lookup failed for {Term}", term);
            }

            // Synonyms
            try
            {
                var synUrl = $"https://api.datamuse.com/words?rel_syn={Uri.EscapeDataString(term)}&max=20";
                var syn = await httpClient.GetFromJsonAsync<List<DatamuseWord>>(synUrl, cancellationToken).ConfigureAwait(false);
                if (syn is not null) response.Synonyms.AddRange(syn.Select(x => x.Word));
            }
            catch (Exception ex)
            {
                logger.LogDebug(ex, "datamuse synonyms lookup failed for {Term}", term);
            }

            // Antonyms
            try
            {
                var antUrl = $"https://api.datamuse.com/words?rel_ant={Uri.EscapeDataString(term)}&max=20";
                var ant = await httpClient.GetFromJsonAsync<List<DatamuseWord>>(antUrl, cancellationToken).ConfigureAwait(false);
                if (ant is not null) response.Antonyms.AddRange(ant.Select(x => x.Word));
            }
            catch (Exception ex)
            {
                logger.LogDebug(ex, "datamuse antonyms lookup failed for {Term}", term);
            }

            // Related terms
            try
            {
                var relUrl = $"https://api.datamuse.com/words?rel_trg={Uri.EscapeDataString(term)}&max=20";
                var rel = await httpClient.GetFromJsonAsync<List<DatamuseWord>>(relUrl, cancellationToken).ConfigureAwait(false);
                if (rel is not null) response.RelatedTerms.AddRange(rel.Select(x => x.Word));
            }
            catch (Exception ex)
            {
                logger.LogDebug(ex, "datamuse related lookup failed for {Term}", term);
            }

            // Translation using LibreTranslate public instance
            try
            {
                var target = MapLanguageToCode(request.TranslationLanguage ?? "es");
                var source = request.Language == Domain.Enums.Language.German ? "de" : "en";
                var translateReq = new { q = term, source = source, target = target, format = "text" };
                var ltResp = await httpClient.PostAsJsonAsync("https://libretranslate.com/translate", translateReq, cancellationToken).ConfigureAwait(false);
                if (ltResp.IsSuccessStatusCode)
                {
                    var obj = await ltResp.Content.ReadFromJsonAsync<LibreTranslateResponse>(cancellationToken: cancellationToken).ConfigureAwait(false);
                    if (obj is not null && !string.IsNullOrWhiteSpace(obj.TranslatedText))
                    {
                        response.Translations.Add(new VocabularyLookupTranslationResponse { Language = request.TranslationLanguage, Text = obj.TranslatedText });
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogDebug(ex, "LibreTranslate lookup failed for {Term}", term);
            }
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "DatamuseTranslate provider failed for {Term}", term);
            throw new InvalidOperationException("El proveedor combinado falló.", ex);
        }

        // Normalize minimal fields
        response.Synonyms = response.Synonyms.Distinct(StringComparer.OrdinalIgnoreCase).ToList();
        response.Antonyms = response.Antonyms.Distinct(StringComparer.OrdinalIgnoreCase).ToList();
        response.RelatedTerms = response.RelatedTerms.Distinct(StringComparer.OrdinalIgnoreCase).ToList();

        return response;
    }

    private static string MapLanguageToCode(string language)
    {
        return language.ToLowerInvariant() switch
        {
            "spanish" or "es" or "español" => "es",
            "german" or "de" or "deutsch" => "de",
            "english" or "en" => "en",
            "french" or "fr" => "fr",
            _ => "es",
        };
    }

    private sealed class DatamuseWord { public string Word { get; set; } = ""; public int Score { get; set; } }
    private sealed class LibreTranslateResponse { public string TranslatedText { get; set; } = ""; }
}
