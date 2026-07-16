using Api.Modules.Files.Application.Common;
using Api.Modules.Files.Application.UseCases.GetConversionStatusUseCase;
using Api.Modules.Files.Application.UseCases.RequestFileConversionUseCase;
using Api.Modules.Files.Presentation.Requests;
using Api.Modules.Files.Presentation.Responses;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Api.Modules.Files.Presentation;

public class ConversionsController : ControllerBase
{
    private readonly IRequestFileConversionUseCaseHandler 
        _requestFileConversionUseCaseHandler;

    private readonly IGetConversionStatusUseCaseHandler 
        _getConversionStatusUseCaseHandler;

    public ConversionsController(
        IRequestFileConversionUseCaseHandler requestHandler,
        IGetConversionStatusUseCaseHandler statusHandler)
    {
        _requestFileConversionUseCaseHandler = requestHandler;
        _getConversionStatusUseCaseHandler = statusHandler;
    }

    public async Task<IActionResult> CreateAsync(
        [FromForm] CreateConversionRequestDto req,
        CancellationToken cancellationToken)
    {
        var result = await _requestFileConversionUseCaseHandler.ExecuteAsync(new RequestFileConversionCommand
        {
            CallerId = GetCurrentUserId(),
            SourceFile = IncomingFile.Create(
                req.File.FileName,
                req.File.ContentType,
                req.File.Length,
                req.File.OpenReadStream),
            TargetFormat = req.TargetFormat,
        }, cancellationToken);

        /*var response = new CreateConversionResponseDto
        {
            ConversionId = result.ConversionId,
            Status = result.Status,
            StatusUrl = "",
            CreatedAt = result.CreatedAt
        };*/

        return Accepted();
    }

    private Guid GetCurrentUserId()
    {
        var val = User.FindFirstValue(ClaimTypes.NameIdentifier);

        return Guid.TryParse(val, out var id) ? id : throw new Exception();
    }
}