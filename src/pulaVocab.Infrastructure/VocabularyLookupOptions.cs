namespace pulaVocab.Infrastructure;

public sealed class VocabularyLookupOptions
{
    public string? ProviderEndpoint { get; set; }
    public string? ApiKey { get; set; }
    public string ApiKeyHeaderName { get; set; } = "Authorization";
    public string ApiKeyScheme { get; set; } = "Bearer";
}
