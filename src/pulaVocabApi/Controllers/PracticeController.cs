using Microsoft.AspNetCore.Mvc;
using pulaVocab.Application.Practice;
using pulaVocab.Domain.Enums;

namespace pulaVocabApi.Controllers;

[ApiController, Route("api/practice")]
public sealed class PracticeController(IPracticeService service, ILogger<PracticeController> logger) : ControllerBase
{
    [HttpGet("statistics")] public Task<PracticeStatisticsResponse> Statistics(Language language, CancellationToken ct) => service.GetStatisticsAsync(language, ct);
    [HttpPost("preview")] public Task<IReadOnlyList<PracticeWordResponse>> Preview(PracticeFilterRequest request, CancellationToken ct) => service.PreviewAsync(request, ct);
    [HttpPost("sessions")] public async Task<ActionResult<PracticeSessionResponse>> Start(StartPracticeRequest request, CancellationToken ct) { try { return Ok(await service.StartAsync(request, ct)); } catch (Exception ex) { logger.LogError(ex, "No se pudo iniciar la práctica"); return Problem("No se pudo iniciar la sesión de práctica."); } }
    [HttpPost("sessions/{id:guid}/answers")] public async Task<ActionResult<PracticeSessionResponse>> Answer(Guid id, SubmitPracticeAnswerRequest request, CancellationToken ct) { try { return await service.SubmitAsync(id, request, ct) is { } value ? Ok(value) : NotFound(); } catch (InvalidOperationException ex) { logger.LogWarning(ex, "Respuesta rechazada en {SessionId}", id); return Conflict(new { message = ex.Message }); } catch (Exception ex) { logger.LogError(ex, "Error guardando respuesta en {SessionId}", id); return Problem("No se pudo guardar la respuesta."); } }
    [HttpPost("sessions/{id:guid}/finish")] public async Task<ActionResult<PracticeSessionResponse>> Finish(Guid id, FinishPracticeRequest request, CancellationToken ct) => await service.FinishAsync(id, request, ct) is { } value ? Ok(value) : NotFound();
}
