using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using Api.Application.Features.Files.ConvertFile;
using Api.Application.Features.Files.DeleteFile;
using Api.Application.Features.Files.FetchFile;

using Api.DTOs.Requests;
using Api.DTOs.Responses;


namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FilesController : ControllerBase
{
    private readonly IConvertFileService _convertFileService;
    private readonly IDeleteFileService _deleteFileService;
    private readonly IFetchFileService _fetchFileService;

    public FilesController(
        IConvertFileService convertFileService,
        IDeleteFileService deleteFileService,
        IFetchFileService fetchFileService
    )
    {
        _convertFileService = convertFileService;
        _deleteFileService = deleteFileService;
        _fetchFileService = fetchFileService;
    }


    [Authorize]
    [HttpGet("{fileId}")]
    public async Task<IActionResult> GetFileAsync([FromRoute] Guid fileId)
    {
        var callerId = GetCurrentUserGuid()!.Value;

        var serviceResult = await _fetchFileService.GetFileAsync(new FetchFileRequest
        {
            FileId = fileId,
            CallerId = callerId
        });

        return File(
            serviceResult.FileStream,
            serviceResult.ContentType,
            serviceResult.FileId.ToString()
        );
    }

    [Authorize]
    [HttpPost("convert")]
    public async Task<IActionResult> ConvertFileAsync([FromForm] ConvertFileRequestDto req)
    {
        var callerId = GetCurrentUserGuid()!.Value;

        var serviceResult = await _convertFileService.ConvertFileAsync(new ConvertFileRequest
        {
            OpenStream = () => req.FormFile.OpenReadStream(),
            TargetExtension = req.TargetExtension,
            ContentType = req.FormFile.ContentType,
            FileName = req.FormFile.FileName,
            CallerId = callerId
        });

        var fileId = serviceResult.FileId.ToString();

        return Created($"/api/Files/{fileId}", new ConvertFileResponseDto
        {
            FileId = fileId,
            ContentType = serviceResult.ContentType,
            FileName = serviceResult.FileName,
            SizeByets = serviceResult.SizeBytes,
            DownloadUrl = serviceResult.DownloadUrl
        });
    }

    [Authorize]
    [HttpDelete("{fileId}")]
    public async Task<IActionResult> DeleteFileAsync([FromRoute] Guid fileId)
    {
        throw new NotImplementedException();
    }


    private Guid? GetCurrentUserGuid()
    {
        var userGuidStr = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

        return Guid.TryParse(userGuidStr, out var guid) ? guid : null;
    }

}