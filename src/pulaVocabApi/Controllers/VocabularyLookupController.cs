using Microsoft.AspNetCore.Mvc;
using pulaVocab.Application.Vocabulary;

namespace pulaVocabApi.Controllers;

[ApiController]
[Route("api/vocabulary")]
public sealed class VocabularyLookupController(IVocabularyService vocabularyService, IVocabularyLookupService lookupService, ILogger<VocabularyLookupController> logger) : ControllerBase
{
    [HttpPost("autocomplete")]
    public async Task<ActionResult<VocabularyLookupResponse>> Autocomplete(VocabularyLookupRequest request, CancellationToken cancellationToken)
    {
        try
        {
            if (request is null) return BadRequest(new { message = "La solicitud es obligatoria." });
            if (string.IsNullOrWhiteSpace(request.Term)) return BadRequest(new { message = "La palabra es obligatoria." });
            var normalizedTerm = request.Term.Trim();

            var existing = await vocabularyService.FindByTermAsync(normalizedTerm, request.Language, cancellationToken);
            if (existing is not null)
            {
                return Ok(new VocabularyLookupResponse
                {
                    Term = existing.Term,
                    NormalizedTerm = existing.Term.Trim().ToLowerInvariant(),
                    Language = existing.Language,
                    PartOfSpeech = existing.PartOfSpeech,
                    Level = existing.Level,
                    Ipa = existing.Ipa,
                    IpaAmerican = existing.IpaAmerican,
                    IpaBritish = existing.IpaBritish,
                    Definitions = existing.Meanings.Where(x => !string.IsNullOrWhiteSpace(x.Definition)).Select(x => new VocabularyLookupDefinitionResponse { Language = existing.Language.ToString(), Text = x.Definition! }).ToList(),
                    Translations = existing.Meanings.Select(x => new VocabularyLookupTranslationResponse { Language = "Spanish", Text = x.Translation }).ToList(),
                    Examples = existing.Examples.Select(x => new VocabularyLookupExampleResponse { Sentence = x.Sentence, Translation = x.Translation }).ToList(),
                    Synonyms = string.IsNullOrWhiteSpace(existing.Synonyms) ? new() : existing.Synonyms.Split(',').Select(x => x.Trim()).Where(x => x.Length > 0).ToList(),
                    Antonyms = string.IsNullOrWhiteSpace(existing.Antonyms) ? new() : existing.Antonyms.Split(',').Select(x => x.Trim()).Where(x => x.Length > 0).ToList(),
                    RelatedTerms = string.IsNullOrWhiteSpace(existing.RelatedTerms) ? new() : existing.RelatedTerms.Split(',').Select(x => x.Trim()).Where(x => x.Length > 0).ToList(),
                    Notes = existing.PersonalNotes,
                    AlreadyExists = true,
                    ExistingEntryId = existing.Id,
                    Message = "Esta palabra ya se encuentra registrada."
                });
            }

            var response = await lookupService.GetLookupAsync(request, cancellationToken);
            return Ok(response);
        }
        catch (ArgumentException ex)
        {
            logger.LogWarning(ex, "Validación de autocompletado fallida para término '{Term}'", request?.Term);
            return BadRequest(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            logger.LogWarning(ex, "Error de proveedor de autocompletado para término '{Term}'", request?.Term);
            return BadRequest(new { message = ex.Message });
        }
        catch (HttpRequestException ex)
        {
            logger.LogWarning(ex, "Error de red en proveedor de autocompletado para término '{Term}'", request?.Term);
            return BadRequest(new { message = "No se pudo contactar con el proveedor de autocompletado." });
        }
        catch (OperationCanceledException)
        {
            return StatusCode(StatusCodes.Status408RequestTimeout, new { message = "La solicitud fue cancelada." });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error inesperado en autocomplete para término '{Term}'", request?.Term);
            return Problem(title: "Error inesperado", detail: "No se pudo obtener información en este momento.", statusCode: StatusCodes.Status500InternalServerError);
        }
    }
}
