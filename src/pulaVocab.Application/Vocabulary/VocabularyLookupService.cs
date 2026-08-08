using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace pulaVocab.Application.Vocabulary;

public sealed class VocabularyLookupService : IVocabularyLookupService
{
    private readonly IVocabularyLookupProvider provider;

    public VocabularyLookupService(IVocabularyLookupProvider provider)
    {
        this.provider = provider ?? throw new ArgumentNullException(nameof(provider));
    }

    public async Task<VocabularyLookupResponse> GetLookupAsync(VocabularyLookupRequest request, CancellationToken cancellationToken = default)
    {
        ValidateRequest(request);

        var response = await provider.LookupAsync(request, cancellationToken).ConfigureAwait(false);
        if (response is null)
        {
            throw new InvalidOperationException("El proveedor no devolvió una respuesta válida.");
        }

        NormalizeResponse(response, request);
        return response;
    }

    private static void ValidateRequest(VocabularyLookupRequest request)
    {
        if (request is null) throw new ArgumentNullException(nameof(request));
        if (string.IsNullOrWhiteSpace(request.Term)) throw new ArgumentException("El término es obligatorio.", nameof(request.Term));
        if (request.Term.Length > 200) throw new ArgumentException("El término no puede exceder 200 caracteres.", nameof(request.Term));
        if (string.IsNullOrWhiteSpace(request.TranslationLanguage)) throw new ArgumentException("El idioma de traducción es obligatorio.", nameof(request.TranslationLanguage));
    }

    private static void NormalizeResponse(VocabularyLookupResponse response, VocabularyLookupRequest request)
    {
        response.Term = NormalizeText(response.Term ?? request.Term);
        response.NormalizedTerm = NormalizeText(response.NormalizedTerm ?? response.Term);
        response.Language = request.Language;
        response.Ipa = NormalizeIpa(response.Ipa);
        response.IpaAmerican = NormalizeIpa(response.IpaAmerican);
        response.IpaBritish = NormalizeIpa(response.IpaBritish);
        response.Definitions = response.Definitions?.Where(x => !string.IsNullOrWhiteSpace(x.Text)).Select(x => new VocabularyLookupDefinitionResponse { Language = NormalizeText(x.Language ?? request.Language.ToString()), Text = NormalizeText(x.Text) }).ToList() ?? new();
        response.Translations = response.Translations?.Where(x => !string.IsNullOrWhiteSpace(x.Text)).Select(x => new VocabularyLookupTranslationResponse { Language = NormalizeText(x.Language ?? request.TranslationLanguage), Text = NormalizeText(x.Text) }).ToList() ?? new();
        response.Examples = response.Examples?.Where(x => !string.IsNullOrWhiteSpace(x.Sentence)).Select(x => new VocabularyLookupExampleResponse { Sentence = NormalizeText(x.Sentence), Translation = NormalizeText(x.Translation) }).ToList() ?? new();
        response.Synonyms = NormalizeList(response.Synonyms);
        response.Antonyms = NormalizeList(response.Antonyms);
        response.RelatedTerms = NormalizeList(response.RelatedTerms);
        response.PhrasalVerbs = response.PhrasalVerbs?.Where(x => !string.IsNullOrWhiteSpace(x.Text)).Select(x => new VocabularyLookupPhrasalVerbResponse { Text = NormalizeText(x.Text), Description = NormalizeText(x.Description) }).ToList() ?? new();
        response.Notes = NormalizeText(response.Notes);
        response.Explanation = NormalizeText(response.Explanation);
        response.Infinitive = NormalizeText(response.Infinitive);
        response.Past = NormalizeText(response.Past);
        response.PastParticiple = NormalizeText(response.PastParticiple);
        response.ThirdPerson = NormalizeText(response.ThirdPerson);
        response.Gerund = NormalizeText(response.Gerund);
    }

    private static string NormalizeText(string? value) => string.IsNullOrWhiteSpace(value) ? null : RemoveMarkdown(value).Trim();
    private static string NormalizeIpa(string? value) => string.IsNullOrWhiteSpace(value) ? null : RemoveMarkdown(value).Trim();
    private static List<string> NormalizeList(List<string>? values) => values?.Where(x => !string.IsNullOrWhiteSpace(x)).Select(NormalizeText).Where(x => x is not null).Distinct(StringComparer.OrdinalIgnoreCase).ToList() ?? new();

    private static string RemoveMarkdown(string value)
    {
        return value.Replace("**", string.Empty)
                    .Replace("*", string.Empty)
                    .Replace("__", string.Empty)
                    .Replace("~~", string.Empty)
                    .Replace("`", string.Empty)
                    .Replace("#", string.Empty)
                    .Trim();
    }
}
