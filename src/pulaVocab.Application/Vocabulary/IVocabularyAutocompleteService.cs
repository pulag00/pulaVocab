using System.Threading;
using System.Threading.Tasks;

namespace pulaVocab.Application.Vocabulary;

public interface IVocabularyAutocompleteService
{
    Task<VocabularyLookupResponse> AutocompleteAsync(VocabularyLookupRequest request, CancellationToken cancellationToken = default);
}
