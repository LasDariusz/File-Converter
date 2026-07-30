using Api.Modules.Files.Application.Ports;
using Api.Modules.Files.Domain.Models.Files;
using Api.Shared.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Api.Modules.Files.Infrastructure.Persistence;

public sealed class FilesRepository : IFilesRepository
{
    private readonly FileConverterContext _dbContext;

    public FilesRepository(FileConverterContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<StoredFileModel?> FindUserFileByIdAsync(
        Guid fileId,
        Guid callerId,
        CancellationToken cancellationToken)
    {
        var row = await (
            from file in _dbContext.Files.AsNoTracking()
            join user in _dbContext.Users.AsNoTracking()
                on file.OwnerId equals user.UserId
            where file.PublicId == fileId
                && user.PublicId == callerId
                && file.DeletedAt == null
            select new { File = file, OwnerPublicId = user.PublicId })
            .SingleOrDefaultAsync(cancellationToken);

        return row is null
            ? null
            : ToModel(row.File, row.OwnerPublicId);
    }

    public async Task<IEnumerable<StoredFileModel>> FindUserFilesByUserIdAsync(
        Guid userId,
        CancellationToken cancellationToken)
    {
        var rows = await (
            from file in _dbContext.Files.AsNoTracking()
            join user in _dbContext.Users.AsNoTracking()
                on file.OwnerId equals user.UserId
            where user.PublicId == userId && file.DeletedAt == null
            orderby file.CreatedAt descending
            select new { File = file, OwnerPublicId = user.PublicId })
            .ToListAsync(cancellationToken);

        return rows.Select(row => ToModel(row.File, row.OwnerPublicId));
    }

    private static StoredFileModel ToModel(
        Entities.FileDataEntity entity,
        Guid ownerPublicId)
    {
        var metadata = new FileMetadata
        {
            FileName = entity.FileName,
            SizeBytes = entity.SizeBytes,
            FileFormat = FileFormat.FromExtension(entity.Extension)
        };

        return StoredFileModel.Rehydrate(
            entity.PublicId,
            ownerPublicId,
            metadata,
            entity.StorageKey,
            entity.CreatedAt);
    }
}
