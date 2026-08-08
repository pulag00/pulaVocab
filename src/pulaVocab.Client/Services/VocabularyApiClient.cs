using System.Net;
using System.Net.Http.Json;
using pulaVocab.Application.Vocabulary;
namespace pulaVocab.Client.Services;

public sealed class VocabularyApiClient(HttpClient http) : IVocabularyApiClient
{
    public async Task<PagedResponse<VocabularyListItemResponse>> GetPagedAsync(VocabularyFilterRequest f, CancellationToken ct = default) => await http.GetFromJsonAsync<PagedResponse<VocabularyListItemResponse>>($"api/vocabulary?language={f.Language}&status={f.Status}&level={f.Level}&partOfSpeech={f.PartOfSpeech}&search={Uri.EscapeDataString(f.Search ?? "")}&page={f.Page}&pageSize={f.PageSize}", ct) ?? new();
    public async Task<VocabularyEntryResponse?> GetByIdAsync(Guid id, CancellationToken ct = default) { var r = await http.GetAsync($"api/vocabulary/{id}", ct); return r.StatusCode == HttpStatusCode.NotFound ? null : await Read<VocabularyEntryResponse>(r, ct); }
    public async Task<VocabularyLookupResponse> AutocompleteAsync(VocabularyLookupRequest request, CancellationToken ct = default) => await Send<VocabularyLookupRequest, VocabularyLookupResponse>(HttpMethod.Post, "api/vocabulary/autocomplete", request, ct);
    public async Task<VocabularyEntryResponse> CreateAsync(CreateVocabularyEntryRequest r, CancellationToken ct = default) => await Send<CreateVocabularyEntryRequest, VocabularyEntryResponse>(HttpMethod.Post, "api/vocabulary", r, ct);
    public async Task<VocabularyEntryResponse> UpdateAsync(Guid id, UpdateVocabularyEntryRequest r, CancellationToken ct = default) => await Send<UpdateVocabularyEntryRequest, VocabularyEntryResponse>(HttpMethod.Put, $"api/vocabulary/{id}", r, ct);
    public async Task DeleteAsync(Guid id, CancellationToken ct = default) { var r = await http.DeleteAsync($"api/vocabulary/{id}", ct); if (!r.IsSuccessStatusCode) throw new InvalidOperationException("No se pudo eliminar la palabra."); }
    private async Task<T> Send<TRequest, T>(HttpMethod method, string uri, TRequest body, CancellationToken ct)
    {
        var r = await http.SendAsync(new HttpRequestMessage(method, uri) { Content = JsonContent.Create(body) }, ct);
        return await Read<T>(r, ct);
    }

    private static async Task<T> Read<T>(HttpResponseMessage r, CancellationToken ct)
    {
        if (r.IsSuccessStatusCode)
        {
            return (await r.Content.ReadFromJsonAsync<T>(ct))!;
        }

        string content = string.Empty;
        try
        {
            content = await r.Content.ReadAsStringAsync(ct);
        }
        catch
        {
            // ignore
        }

        if (!string.IsNullOrWhiteSpace(content))
        {
            throw new InvalidOperationException($"Error {r.StatusCode}: {content}");
        }

        throw new InvalidOperationException($"Error {r.StatusCode}: No se pudo completar la operación.");
    }
}
