using System.Threading;
using System.Threading.Tasks;

namespace pulaVocab.Application.Vocabulary;

public interface IVocabularyLookupProvider
{
    Task<VocabularyLookupResponse> LookupAsync(VocabularyLookupRequest request, CancellationToken cancellationToken = default);
}
