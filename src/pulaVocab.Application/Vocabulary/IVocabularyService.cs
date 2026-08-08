using pulaVocab.Domain.Enums;

namespace pulaVocab.Application.Vocabulary;

public interface IVocabularyService
{
    Task<PagedResponse<VocabularyListItemResponse>> GetPagedAsync(VocabularyFilterRequest filter, CancellationToken cancellationToken);
    Task<VocabularyEntryResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<VocabularyEntryResponse?> FindByTermAsync(string term, Language language, CancellationToken cancellationToken);
    Task<VocabularyEntryResponse> CreateAsync(CreateVocabularyEntryRequest request, CancellationToken cancellationToken);
    Task<VocabularyEntryResponse?> UpdateAsync(Guid id, UpdateVocabularyEntryRequest request, CancellationToken cancellationToken);
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken);
}
