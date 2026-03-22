using Api.Database.Context;
using Api.DomainModels.Features.UserFilesModel;
using Api.DomainModels.GenericResult;
using Api.DTOs.Features.FilesMetadata;
using Microsoft.EntityFrameworkCore;

namespace Api.Services.Users;

public class UsersService : IUsersService
{
    private readonly FileConverterContext _dbContext;

    public UsersService(FileConverterContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result<FetchUserFilesModelResponse>> GetUserFilesAsync(FetchUserFilesModel model)
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
    }
}