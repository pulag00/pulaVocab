using pulaVocab.Application.Vocabulary;
namespace pulaVocab.Client.Services;
public interface IVocabularyApiClient
{
    Task<PagedResponse<VocabularyListItemResponse>> GetPagedAsync(VocabularyFilterRequest filter, CancellationToken cancellationToken = default);
    Task<VocabularyEntryResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<VocabularyEntryResponse> CreateAsync(CreateVocabularyEntryRequest request, CancellationToken cancellationToken = default);
    Task<VocabularyEntryResponse> UpdateAsync(Guid id, UpdateVocabularyEntryRequest request, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
