using Microsoft.EntityFrameworkCore;

using Api.Infrastructure.Database.Context;
using Api.Infrastructure.ObjectStorage;

using Api.Application.Exceptions;


namespace Api.Application.Features.Files.FetchFile;

public class FetchFileService : IFetchFileService
{
    private readonly FileConverterContext _dbContext;
    private readonly IObjectStorage _objectStorage;

    public FetchFileService(
        FileConverterContext dbContext,
        IObjectStorage objectStorage)
    {
        _dbContext = dbContext;
        _objectStorage = objectStorage;
    }


    public async Task<FetchFileResponse> GetFileAsync(FetchFileRequest req)
    {
        var fileInfo = await _dbContext.Files
            .Where(f => f.PublicId == req.FileId)
            .Select(f => new
            {
                f.StorageKey,
                f.FileName,
                f.ContentType,
                f.PublicId,
                OwnerPublicId = f.Owner!.PublicId
            })
            .FirstOrDefaultAsync();

        if (fileInfo is null)
        {
            throw new NotFoundException("File", req.FileId.ToString());
        }

        if (fileInfo.OwnerPublicId != req.CallerId)
        {
            throw new UnauthorizedException();
        }

        var stream = await _objectStorage
            .GetOpenFileStreamAsync(fileInfo.StorageKey);

        if (stream is null)
        {
            // !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
        }
     
        return new FetchFileResponse
        {
            FileStream = stream,
            ContentType = fileInfo.ContentType,
            FileId = fileInfo.PublicId
        };
    }

}


/*    private async Task<int?> GetUserIdAsync(Guid userPublicId)
    {
        var userId = await _dbContext.Users
            .Where(u => u.PublicId == userPublicId)
            .Select(u => (int?)u.Id)
            .FirstOrDefaultAsync();

        return userId;
    }*/
/*    public async Task<Result<FetchUserFilesModelResponse>> GetUserFilesAsync(FetchUserFilesModel model)
    {
        var userExists = await _dbContext.Users
            .AnyAsync(u => u.PublicId == model.UserId);

        if (!userExists)
            return Result<FetchUserFilesModelResponse>.Failure(
                $"User {model.UserId} not found.", ErrorType.NotFound);

        var files = await _dbContext.Files
            .Where(f => f.Owner!.PublicId == model.UserId && !f.IsDeleted)
            .OrderByDescending(f => f.CreatedAt)
            .Select(f => new FileMetadataDto
            {
                id = f.PublicId,
                fileName = f.FileName,
                ContentType = f.ContentType,
                FileSizeBytes = f.FileSizeBytes,
                Status = f.Status,
                CreatedAt = f.CreatedAt,
                DownloadUrl = string.Empty,
                SourceFileDownloadUrl = null,
            })
            .AsNoTracking()
            .ToListAsync();

        foreach (var file in files)
            file.DownloadUrl = $"/api/Files/{file.id}";

        return Result<FetchUserFilesModelResponse>.Success(new FetchUserFilesModelResponse
        {
            Files = files
        });
    }*/