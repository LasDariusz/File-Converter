using Microsoft.EntityFrameworkCore;
using Api.Application.Exceptions;
using Api.Application.Common;
using Api.Infrastructure.Database.Context;

namespace Api.Application.Features.Files.FetchFilesMetadata;

public class FetchFilesMetadataService : IFetchFilesMetadataService
{
    private readonly FileConverterContext _dbContext;

    public FetchFilesMetadataService(FileConverterContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<FetchFilesMetadataResponse> GetFilesMetadataAsync(FetchFilesMetadataRequest req)
    {
        if (req.FileId.HasValue)
        {
            var file = await _dbContext.Files
                .Where(f => f.PublicId == req.FileId)
                .Select(f => new
                {
                    f.PublicId,
                    f.FileName,
                    f.ContentType,
                    f.FileSizeBytes,
                    f.Status,
                    f.CreatedAt,
                    SourceFilePublicId = f.SourceFile != null
                        ? f.SourceFile.PublicId : (Guid?)null,
                    OwnerPublicId = f.Owner!.PublicId,
                })
                .FirstOrDefaultAsync() ??
                throw new NotFoundException("File", req.FileId.Value.ToString());


            if (file.OwnerPublicId != req.UserId)
                throw new ForbiddenException();

            return new FetchFilesMetadataResponse
            {
                FilesMetadata = [MapToFileMetadata(file.PublicId, file.FileName, 
                    file.ContentType, file.FileSizeBytes, file.Status, file.CreatedAt,
                    file.SourceFilePublicId)]
            };
        }

        var files = await _dbContext.Files
            .Where(f => f.Owner!.PublicId == req.UserId)
            .Select(f => new
            {
                f.PublicId,
                f.FileName,
                f.ContentType,
                f.FileSizeBytes,
                f.Status,
                f.CreatedAt,
                SourceFilePublicId = f.SourceFile != null
                        ? f.SourceFile.PublicId : (Guid?)null,
            }).ToListAsync();

        return new FetchFilesMetadataResponse
        {
            FilesMetadata = files.Select(f => MapToFileMetadata(f.PublicId, f.FileName,
                    f.ContentType, f.FileSizeBytes, f.Status, f.CreatedAt,
                    f.SourceFilePublicId))
        };
    }

    private static FileMetadata MapToFileMetadata(
        Guid publicId, string fileName, string contentType, long fileSizeBytes, 
        string? status, DateTime createdAt, Guid? sourceFilePublicId)
    {
        string endpoint = "/api/Files/";

        return new FileMetadata
        {
            FileId = publicId,
            FileName = fileName,
            ContentType = contentType,
            FileSizeBytes = fileSizeBytes,
            Status = status,
            CreatedAt = createdAt,
            DownloadUrl = $"{endpoint}{publicId}",
            SourceFileDownloadUrl = sourceFilePublicId != null ?
                $"{endpoint}{sourceFilePublicId}" : null
        };
    }

}