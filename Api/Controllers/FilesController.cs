using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Api.Application.Features.Files.ConvertFile;
using Api.Application.Features.Files.DeleteFile;
using Api.Application.Features.Files.FetchFile;
using Api.Application.Features.Files.FetchFilesMetadata;
using Api.DTOs.Requests;
using Api.DTOs.Responses;
using Api.DTOs.Shared;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FilesController : ControllerBase
{
    private readonly IConvertFileService _convertFileService;
    private readonly IDeleteFileService _deleteFileService;
    private readonly IFetchFileService _fetchFileService;
    private readonly IFetchFilesMetadataService _fetchFilesMetadataService;

    public FilesController(
        IConvertFileService convertFileService,
        IDeleteFileService deleteFileService,
        IFetchFileService fetchFileService,
        IFetchFilesMetadataService fetchFilesMetadataService
    )
    {
        _convertFileService = convertFileService;
        _deleteFileService = deleteFileService;
        _fetchFileService = fetchFileService;
        _fetchFilesMetadataService = fetchFilesMetadataService;
    }

    [Authorize]
    [HttpGet("{fileId}")]
    [ProducesResponseType(typeof(FetchFilesMetadataResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
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
    [HttpGet("metadata")]
    [ProducesResponseType(typeof(FetchFilesMetadataResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetFilesMetadataAsync([FromQuery] FetchFilesMetadataRequestDto req)
    {
        var callerId = GetCurrentUserGuid();

        var serviceResult = await _fetchFilesMetadataService.GetFilesMetadataAsync(
            new FetchFilesMetadataRequest
        {
            FileId = req.FileId,
            UserId = callerId!.Value
        });

        var res = new FetchFilesMetadataResponseDto
        {
            FilesMetadata = serviceResult.FilesMetadata
                .Select(f => new FileMetadataDto
            {
                FileId = f.FileId,
                FileName = f.FileName,
                ContentType = f.ContentType,
                FileSizeBytes = f.FileSizeBytes,
                Status = f.Status,
                CreatedAt = f.CreatedAt,
                DownloadUrl = f.DownloadUrl,
                SourceFileDownloadUrl = f.SourceFileDownloadUrl
            })
        };

        return Ok(res);
    }

    [Authorize]
    [HttpPost("convert")]
    public async Task<IActionResult> ConvertFileAsync(
        [FromForm] ConvertFileRequestDto req, CancellationToken cancellation)
    {
        var callerId = GetCurrentUserGuid()!.Value;

        var serviceResult = await _convertFileService.ConvertFileAsync(new ConvertFileRequest
        {
            OpenStream = () => req.FormFile.OpenReadStream(),
            TargetExtension = req.TargetExtension,
            ContentType = req.FormFile.ContentType,
            FileName = req.FormFile.FileName,
            CallerId = callerId
        }, cancellation);

        return Created(serviceResult.ConvertedFile.DownloadUrl, new ConvertFileResponseDto
        {
            SourceFile = new FileMetadataDto 
            { 
                FileId = serviceResult.SourceFile.FileId,
                FileName = serviceResult.SourceFile.FileName,
                ContentType = serviceResult.SourceFile.ContentType,
                FileSizeBytes= serviceResult.SourceFile.FileSizeBytes,
                Status = serviceResult.SourceFile.Status,
                CreatedAt = serviceResult.SourceFile.CreatedAt,
                DownloadUrl = serviceResult.SourceFile.DownloadUrl,
                SourceFileDownloadUrl = serviceResult.SourceFile.SourceFileDownloadUrl
            },
            ConvertedFile = new FileMetadataDto
            {
                FileId = serviceResult.ConvertedFile.FileId,
                FileName = serviceResult.ConvertedFile.FileName,
                ContentType = serviceResult.ConvertedFile.ContentType,
                FileSizeBytes = serviceResult.ConvertedFile.FileSizeBytes,
                Status = serviceResult.ConvertedFile.Status,
                CreatedAt = serviceResult.ConvertedFile.CreatedAt,
                DownloadUrl = serviceResult.ConvertedFile.DownloadUrl,
                SourceFileDownloadUrl = serviceResult.ConvertedFile.SourceFileDownloadUrl
            }
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