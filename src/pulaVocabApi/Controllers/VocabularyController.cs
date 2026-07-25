using Microsoft.AspNetCore.Mvc;
using pulaVocab.Application.Vocabulary;

namespace pulaVocabApi.Controllers;

[ApiController]
[Route("api/vocabulary")]
public sealed class VocabularyController(IVocabularyService service) : ControllerBase
{
    [HttpGet]
    public Task<PagedResponse<VocabularyListItemResponse>> Get([FromQuery] VocabularyFilterRequest filter, CancellationToken cancellationToken) => service.GetPagedAsync(filter, cancellationToken);

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<VocabularyEntryResponse>> GetById(Guid id, CancellationToken cancellationToken) =>
        await service.GetByIdAsync(id, cancellationToken) is { } entry ? Ok(entry) : NotFound();

    [HttpPost]
    public async Task<ActionResult<VocabularyEntryResponse>> Create(CreateVocabularyEntryRequest request, CancellationToken cancellationToken)
    {
        try { var entry = await service.CreateAsync(request, cancellationToken); return CreatedAtAction(nameof(GetById), new { entry.Id }, entry); }
        catch (Exception ex) when (ex is ArgumentException or InvalidOperationException) { return Problem(title: "No fue posible crear la palabra", detail: ex.Message, statusCode: StatusCodes.Status400BadRequest); }
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<VocabularyEntryResponse>> Update(Guid id, UpdateVocabularyEntryRequest request, CancellationToken cancellationToken)
    {
        try { return await service.UpdateAsync(id, request, cancellationToken) is { } entry ? Ok(entry) : NotFound(); }
        catch (Exception ex) when (ex is ArgumentException or InvalidOperationException) { return Problem(title: "No fue posible actualizar la palabra", detail: ex.Message, statusCode: StatusCodes.Status400BadRequest); }
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken) => await service.DeleteAsync(id, cancellationToken) ? NoContent() : NotFound();
}
