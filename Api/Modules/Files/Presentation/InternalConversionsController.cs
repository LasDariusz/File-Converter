using Api.Modules.Files.Application.Ports;
using Api.Modules.Files.Presentation.Requests;
using Microsoft.AspNetCore.Mvc;

namespace Api.Modules.Files.Presentation;

[ApiController]
[Route("internal/conversions")]
public sealed class InternalConversionsController : ControllerBase
{
    private readonly IConversionsRepository _repository;
    private readonly IConfiguration _configuration;

    public InternalConversionsController(
        IConversionsRepository repository,
        IConfiguration configuration)
    {
        _repository = repository;
        _configuration = configuration;
    }

    [HttpPost("{conversionId:guid}/started")]
    public async Task<IActionResult> StartedAsync(
        [FromRoute] Guid conversionId,
        CancellationToken cancellationToken)
    {
        if (!IsAuthorizedWorker())
            return Unauthorized();

        var result = await _repository.TryStartAsync(
            conversionId,
            cancellationToken);

        return result is null
            ? NotFound()
            : Ok(new
            {
                result.ShouldProcess,
                Status = result.Status.ToString()
            });
    }

    [HttpPost("{conversionId:guid}/completed")]
    public async Task<IActionResult> CompletedAsync(
        [FromRoute] Guid conversionId,
        [FromBody] CompleteConversionRequestDto request,
        CancellationToken cancellationToken)
    {
        if (!IsAuthorizedWorker())
            return Unauthorized();

        var found = await _repository.CompleteAsync(
            conversionId,
            new ConversionCompletion(
                request.OutputStorageKey,
                request.Extension,
                request.ContentType,
                request.SizeBytes),
            cancellationToken);

        return found ? NoContent() : NotFound();
    }

    [HttpPost("{conversionId:guid}/failed")]
    public async Task<IActionResult> FailedAsync(
        [FromRoute] Guid conversionId,
        [FromBody] FailConversionRequestDto request,
        CancellationToken cancellationToken)
    {
        if (!IsAuthorizedWorker())
            return Unauthorized();

        var found = await _repository.FailAsync(
            conversionId,
            request.Error,
            cancellationToken);

        return found ? NoContent() : NotFound();
    }

    private bool IsAuthorizedWorker()
    {
        var expectedKey = _configuration["InternalApi:Key"];
        var suppliedKey = Request.Headers["X-Internal-Key"].ToString();

        return !string.IsNullOrWhiteSpace(expectedKey) &&
            string.Equals(expectedKey, suppliedKey, StringComparison.Ordinal);
    }
}
