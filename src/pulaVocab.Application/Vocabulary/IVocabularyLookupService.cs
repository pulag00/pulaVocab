using System.Threading;
using System.Threading.Tasks;

namespace pulaVocab.Application.Vocabulary;

public interface IVocabularyLookupService
{
    Task<VocabularyLookupResponse> GetLookupAsync(VocabularyLookupRequest request, CancellationToken cancellationToken = default);
}
