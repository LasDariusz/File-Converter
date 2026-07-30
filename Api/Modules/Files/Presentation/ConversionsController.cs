using System.Security.Claims;
using Api.Modules.Files.Application.Common;
using Api.Modules.Files.Application.UseCases.GetConversionStatusUseCase;
using Api.Modules.Files.Application.UseCases.RequestFileConversionUseCase;
using Api.Modules.Files.Presentation.Requests;
using Api.Modules.Files.Presentation.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Modules.Files.Presentation;

[ApiController]
[Authorize]
[Route("api/conversions")]
public sealed class ConversionsController : ControllerBase
{
    private const string GetConversionStatusRoute = "GetConversionStatus";

    private readonly IRequestFileConversionUseCaseHandler _requestHandler;
    private readonly IGetConversionStatusUseCaseHandler _statusHandler;

    public ConversionsController(
        IRequestFileConversionUseCaseHandler requestHandler,
        IGetConversionStatusUseCaseHandler statusHandler)
    {
        _requestHandler = requestHandler;
        _statusHandler = statusHandler;
    }

    [HttpPost]
    [ProducesResponseType(typeof(CreateConversionResponseDto), StatusCodes.Status202Accepted)]
    public async Task<IActionResult> CreateAsync(
        [FromForm] CreateConversionRequestDto request,
        CancellationToken cancellationToken)
    {
        var result = await _requestHandler.ExecuteAsync(
            new RequestFileConversionCommand
            {
                CallerId = GetCurrentUserId(),
                File = IncomingFile.Create(
                    request.File.FileName,
                    request.File.ContentType,
                    request.File.Length,
                    request.File.OpenReadStream),
                TargetFormat = request.TargetFormat
            },
            cancellationToken);

        var response = new CreateConversionResponseDto
        {
            ConversionId = result.ConversionId,
            Status = (ConversionStatusDto)result.Status,
            StatusUrl = Url.Link(
                GetConversionStatusRoute,
                new { conversionId = result.ConversionId }) ??
                $"/api/conversions/{result.ConversionId}",
            CreatedAt = result.CreatedAt
        };

        Response.Headers.Location = response.StatusUrl;
        return Accepted(response);
    }

    [HttpGet("{conversionId:guid}", Name = GetConversionStatusRoute)]
    [ProducesResponseType(typeof(GetConversionStatusResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetStatusAsync(
        [FromRoute] Guid conversionId,
        CancellationToken cancellationToken)
    {
        var result = await _statusHandler.ExecuteAsync(
            conversionId,
            GetCurrentUserId(),
            cancellationToken);

        if (result is null)
            return NotFound();

        return Ok(new GetConversionStatusResponseDto
        {
            ConversionId = result.ConversionId,
            Status = (ConversionStatusDto)result.Status,
            OutputFileId = result.OutputFileId,
            Error = result.ErrorMessage,
            CreatedAt = result.CreatedAt,
            StartedAt = result.StartedAt,
            FinishedAt = result.FinishedAt
        });
    }

    private Guid GetCurrentUserId()
    {
        var value = User.FindFirstValue(ClaimTypes.NameIdentifier);

        return Guid.TryParse(value, out var id)
            ? id
            : throw new UnauthorizedAccessException("Missing user identifier.");
    }
}
