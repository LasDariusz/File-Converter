using Api.Modules.Files.Application.Ports;
using Api.Modules.Files.Domain.Models.Conversions;
using Api.Modules.Files.Domain.Models.Files;
using Api.Modules.Files.Infrastructure.Persistence.Entities;
using Api.Shared.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Api.Modules.Files.Infrastructure.Persistence;

public sealed class ConversionsRepository : IConversionsRepository
{
    private readonly FileConverterContext _dbContext;

    public ConversionsRepository(FileConverterContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<ConversionModel> CreateWithSourceFileAsync(
        ConversionModel conversion,
        StoredFileModel sourceFile,
        CancellationToken cancellationToken)
    {
        if (conversion.OwnerId != sourceFile.OwnerId ||
            conversion.SourceFileId != sourceFile.FileId)
        {
            throw new InvalidOperationException(
                "The source file does not belong to the conversion.");
        }

        var ownerInternalId = await _dbContext.Users
            .Where(user => user.PublicId == conversion.OwnerId)
            .Select(user => (int?)user.UserId)
            .SingleOrDefaultAsync(cancellationToken)
            ?? throw new InvalidOperationException("Conversion owner was not found.");

        await using var transaction = await _dbContext.Database
            .BeginTransactionAsync(cancellationToken);

        var sourceEntity = new FileDataEntity
        {
            PublicId = sourceFile.FileId,
            OwnerId = ownerInternalId,
            FileName = sourceFile.FileName,
            ContentType = sourceFile.ContentType,
            Extension = sourceFile.Extension,
            SizeBytes = sourceFile.SizeBytes,
            StorageKey = sourceFile.StorageKey,
            CreatedAt = sourceFile.CreatedAt
        };

        await _dbContext.Files.AddAsync(sourceEntity, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

        var conversionEntity = new ConversionJobEntity
        {
            PublicId = conversion.ConversionId,
            OwnerId = ownerInternalId,
            SourceFileId = sourceEntity.FileId,
            SourceFormat = conversion.SourceFormat.Extension,
            OutputFormat = conversion.OutputFormat.Extension,
            Status = ConversionStatus.Queued,
            AttemptCount = 0,
            CreatedAt = conversion.CreatedAt
        };

        await _dbContext.Conversions.AddAsync(
            conversionEntity,
            cancellationToken);

        await _dbContext.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        return conversion;
    }

    public async Task<ConversionModel?> FindForOwnerAsync(
        Guid conversionId,
        Guid ownerId,
        CancellationToken cancellationToken)
    {
        var row = await (
            from conversion in _dbContext.Conversions.AsNoTracking()
            join owner in _dbContext.Users.AsNoTracking()
                on conversion.OwnerId equals owner.UserId
            join source in _dbContext.Files.AsNoTracking()
                on conversion.SourceFileId equals source.FileId
            where conversion.PublicId == conversionId
                && owner.PublicId == ownerId
            select new
            {
                Conversion = conversion,
                OwnerPublicId = owner.PublicId,
                SourcePublicId = source.PublicId
            })
            .SingleOrDefaultAsync(cancellationToken);

        if (row is null)
            return null;

        Guid? outputPublicId = null;

        if (row.Conversion.OutputFileId is int outputInternalId)
        {
            outputPublicId = await _dbContext.Files
                .Where(file => file.FileId == outputInternalId)
                .Select(file => (Guid?)file.PublicId)
                .SingleOrDefaultAsync(cancellationToken);
        }

        return ConversionModel.Rehydrate(
            row.Conversion.PublicId,
            row.OwnerPublicId,
            row.SourcePublicId,
            outputPublicId,
            FileFormat.FromExtension(row.Conversion.SourceFormat),
            FileFormat.FromExtension(row.Conversion.OutputFormat),
            row.Conversion.Status,
            row.Conversion.ErrorMessage,
            row.Conversion.AttemptCount,
            row.Conversion.CreatedAt,
            row.Conversion.StartedAt,
            row.Conversion.FinishedAt);
    }

    public async Task<ConversionStartResult?> TryStartAsync(
        Guid conversionId,
        CancellationToken cancellationToken)
    {
        var conversion = await _dbContext.Conversions
            .SingleOrDefaultAsync(
                job => job.PublicId == conversionId,
                cancellationToken);

        if (conversion is null)
            return null;

        if (conversion.Status is ConversionStatus.Completed
            or ConversionStatus.Failed
            or ConversionStatus.Cancelled)
        {
            return new ConversionStartResult(false, conversion.Status);
        }

        conversion.Status = ConversionStatus.Processing;
        conversion.StartedAt ??= DateTime.UtcNow;
        conversion.AttemptCount++;

        await _dbContext.SaveChangesAsync(cancellationToken);

        return new ConversionStartResult(true, conversion.Status);
    }

    public async Task<bool> CompleteAsync(
        Guid conversionId,
        ConversionCompletion completion,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(completion.OutputStorageKey) ||
            completion.SizeBytes <= 0)
        {
            throw new ArgumentException("Invalid conversion output metadata.");
        }

        var outputFormat = FileFormat.FromExtension(completion.Extension);

        await using var transaction = await _dbContext.Database
            .BeginTransactionAsync(cancellationToken);

        var conversion = await _dbContext.Conversions
            .SingleOrDefaultAsync(
                job => job.PublicId == conversionId,
                cancellationToken);

        if (conversion is null)
            return false;

        if (conversion.Status == ConversionStatus.Completed)
            return true;

        if (conversion.Status is ConversionStatus.Failed or ConversionStatus.Cancelled)
            return true;

        if (!string.Equals(
            conversion.OutputFormat,
            outputFormat.Extension,
            StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException(
                "Worker output format does not match the conversion request.");
        }

        var sourceFile = await _dbContext.Files
            .SingleAsync(
                file => file.FileId == conversion.SourceFileId,
                cancellationToken);

        var outputFile = await _dbContext.Files
            .SingleOrDefaultAsync(
                file => file.StorageKey == completion.OutputStorageKey,
                cancellationToken);

        if (outputFile is null)
        {
            outputFile = new FileDataEntity
            {
                PublicId = Guid.CreateVersion7(),
                OwnerId = conversion.OwnerId,
                FileName = BuildOutputFileName(sourceFile.FileName, outputFormat.Extension),
                ContentType = completion.ContentType,
                Extension = outputFormat.Extension,
                SizeBytes = completion.SizeBytes,
                StorageKey = completion.OutputStorageKey,
                CreatedAt = DateTime.UtcNow
            };

            await _dbContext.Files.AddAsync(outputFile, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        conversion.OutputFileId = outputFile.FileId;
        conversion.Status = ConversionStatus.Completed;
        conversion.ErrorMessage = null;
        conversion.FinishedAt = DateTime.UtcNow;

        await _dbContext.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        return true;
    }

    public async Task<bool> FailAsync(
        Guid conversionId,
        string errorMessage,
        CancellationToken cancellationToken)
    {
        var conversion = await _dbContext.Conversions
            .SingleOrDefaultAsync(
                job => job.PublicId == conversionId,
                cancellationToken);

        if (conversion is null)
            return false;

        if (conversion.Status == ConversionStatus.Completed)
            return true;

        conversion.Status = ConversionStatus.Failed;
        conversion.ErrorMessage = string.IsNullOrWhiteSpace(errorMessage)
            ? "Conversion failed."
            : errorMessage[..Math.Min(errorMessage.Length, 255)];
        conversion.FinishedAt = DateTime.UtcNow;

        await _dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }

    private static string BuildOutputFileName(
        string sourceFileName,
        string outputExtension)
    {
        var baseName = Path.GetFileNameWithoutExtension(sourceFileName);

        if (string.IsNullOrWhiteSpace(baseName))
            baseName = "converted";

        return $"{baseName}.{outputExtension}";
    }
}
