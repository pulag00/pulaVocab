using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.Extensions.Options;
using pulaVocab.Application.Vocabulary;

namespace pulaVocab.Infrastructure;

public sealed class HttpVocabularyLookupProvider : IVocabularyLookupProvider
{
    private readonly HttpClient httpClient;
    private readonly VocabularyLookupOptions options;

    public HttpVocabularyLookupProvider(HttpClient httpClient, IOptions<VocabularyLookupOptions> options)
    {
        this.httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        this.options = options?.Value ?? throw new ArgumentNullException(nameof(options));
    }

    public async Task<VocabularyLookupResponse> LookupAsync(VocabularyLookupRequest request, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(options.ProviderEndpoint))
        {
            throw new InvalidOperationException("No se ha configurado el proveedor de autocompletado.");
        }

        var message = new HttpRequestMessage(HttpMethod.Post, options.ProviderEndpoint)
        {
            Content = JsonContent.Create(request)
        };

        if (!string.IsNullOrWhiteSpace(options.ApiKey))
        {
            if (string.Equals(options.ApiKeyHeaderName, "Authorization", StringComparison.OrdinalIgnoreCase))
            {
                message.Headers.Authorization = new AuthenticationHeaderValue(options.ApiKeyScheme, options.ApiKey);
            }
            else
            {
                message.Headers.Add(options.ApiKeyHeaderName, options.ApiKey);
            }
        }

        HttpResponseMessage response;

        try
        {
            response = await httpClient.SendAsync(message, cancellationToken).ConfigureAwait(false);
        }
        catch (HttpRequestException ex)
        {
            throw new InvalidOperationException("No se pudo contactar con el proveedor de autocompletado.", ex);
        }

        if (!response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
            throw new InvalidOperationException($"El proveedor de autocompletado respondió con {(int)response.StatusCode} {response.ReasonPhrase}. {content}");
        }

        var result = await response.Content.ReadFromJsonAsync<VocabularyLookupResponse>(cancellationToken: cancellationToken);
        return result ?? throw new InvalidOperationException("El proveedor no devolvió datos.");
    }
}
