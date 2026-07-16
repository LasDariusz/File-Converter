using Api.Modules.Files.Application.UseCases.GetFileDetailsUseCase;
using Api.Modules.Files.Application.UseCases.GetFileUseCase;
using Api.Modules.Files.Presentation.Requests;
using Api.Modules.Files.Presentation.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Api.Modules.Files.Presentation;

[ApiController]
[Route("api/[controller]")]
public class FilesController : ControllerBase
{
    private readonly IGetFileUseCaseHandler _getFileUseCaseHandler;
    private readonly IGetUserFilesMetadataUseCaseHandler _getUserFilesMetadataUseCaseHandler;

    public FilesController(
        IGetFileUseCaseHandler getFileUseCaseHandler,
        IGetUserFilesMetadataUseCaseHandler getUserFilesMetadataUseCaseHandler)
    {
        _getFileUseCaseHandler = getFileUseCaseHandler;
        _getUserFilesMetadataUseCaseHandler = getUserFilesMetadataUseCaseHandler;
    }

    [Authorize]
    [HttpGet("{fileId}")]
    [ProducesResponseType(typeof(File), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetFileAsync(
        [FromRoute] Guid fileId, 
        CancellationToken cancellationToken)
    {
        var result = await _getFileUseCaseHandler.ExecuteAsync(new GetFileCommand
        {
            FileId = fileId,
            CallerId = GetCurrentUserId(),
        }, cancellationToken);

        return File(
            result.FileStream,
            result.ContentType,
            result.FileId.ToString()
        );
    }

    [Authorize]
    [HttpGet("metadata")]
    [ProducesResponseType(typeof(GetFilesMetadataResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetFilesMetadataAsync(
        [FromQuery] Guid userId,
        CancellationToken cancellationToken)
    {
        /*var result = await _getUserFilesMetadataUseCaseHandler.ExecuteAsync(new GetUserFilesMetadataCommand
        {
            UserId = userId,
            CallerId = GetCurrentUserId()
        }, cancellationToken);

        var response = new GetFilesMetadataResponseDto 
        { 
            FilesMetadata = result.FilesMetadata
        };
*/
        return Ok();
    }

    [Authorize]
    [HttpDelete("{fileId}")]
    public async Task<IActionResult> DeleteFileAsync([FromRoute] Guid fileId)
    {
        throw new NotImplementedException();
    }

    private Guid GetCurrentUserId()
    {
        var val = User.FindFirstValue(ClaimTypes.NameIdentifier);

        return Guid.TryParse(val, out var id) ? id : throw new Exception();
    }

}