using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using Api.Services.Files;
using Api.DomainModels.GenericResult;
using Api.DomainModels.Features.ConvertFile;
using Api.DomainModels.Features.FetchFile;
using Api.DomainModels.Features.UploadFile;
using Api.DTOs.Features.ConvertFile;
using Api.DTOs.Features.UploadFile;


namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FilesController : ControllerBase
{
    private readonly IFilesService _filesService;

    public FilesController(IFilesService filesService)
    {
        _filesService = filesService;
    }


    private Guid? GetCurrentUserGuid()
    {
        var userGuidString = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

        var parsed = Guid.TryParse(userGuidString, out var userGuid);

        if (!parsed)
        {
            return null;
        }

        return userGuid;
    }

    private IActionResult MapError(ErrorType? errorType, string? error)
    {
        return errorType switch
        {
            ErrorType.NotFound => NotFound(error),

            ErrorType.Validation => BadRequest(error),

            ErrorType.Conflict => Conflict(error),

            ErrorType.Unauthorized => Unauthorized(error),

            ErrorType.Unknown => StatusCode(500, error),

            _ => StatusCode(500, error)
        };
    }


    [Authorize]
    [HttpGet("{id}")]
    public async Task<IActionResult> GetFileAsync([FromRoute] Guid id)
    {
        var userGuid = GetCurrentUserGuid();

        if (userGuid == null) return Unauthorized();
        
        var serviceResult = await _filesService.GetFileByGuidAsync(new FetchFileModel 
        {
            FileId = id,
            UserId = userGuid.Value
        });

        if (!serviceResult.IsSuccess)
        {
            return MapError(serviceResult.ErrorType, serviceResult.Error);
        }

        var file = serviceResult.Value!;

        return File(file.FileStream, file.ContentType, file.FileName);
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> PostFileAsync([FromForm] CreateNewFileRequestDto req)
    {
        var userGuid = GetCurrentUserGuid();

        if (userGuid == null) return Unauthorized();
        
        var serviceResult = await _filesService.UploadFileAsync(new UploadFileModel
        {
            OpenStream = () => req.FormFile.OpenReadStream(),
            ContentType = req.FormFile.ContentType,
            FileName = req.FormFile.FileName,
            LengthBytes = req.FormFile.Length
        });

        if (!serviceResult.IsSuccess)
        {
            return MapError(serviceResult.ErrorType, serviceResult.Error);
        }

        return Ok(serviceResult.Value);
    }

    [Authorize]
    [HttpPost("convert")]
    public async Task<IActionResult> ConvertFileAsync([FromForm] ConvertFileRequestDto req)
    {
        var userGuid = GetCurrentUserGuid();

        if (userGuid == null) return Unauthorized();
        
        var serviceResult = await _filesService.ConvertFileToSpecifiedFormatAsync(new ConvertFileModel
        {
            UserId = userGuid.Value,
            OpenStream = () => req.FormFile.OpenReadStream(),
            FileName = req.FormFile.FileName,
            TargetExtension = req.TargetFormat,
            ContentType = req.FormFile.ContentType
        });

        if (!serviceResult.IsSuccess)
        {
            return MapError(serviceResult.ErrorType, serviceResult.Error);
        }

        var file = serviceResult.Value!;

        return File(file.OutputStream, file.ContentType);
    }

    [Authorize]
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteFileAsync()
    {
        throw new NotImplementedException();
    }

}
