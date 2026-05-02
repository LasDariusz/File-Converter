using Microsoft.EntityFrameworkCore;
using Api.Application.Validation;
using Api.Application.Exceptions;
using Api.Application.Utils;
using Api.Infrastructure.ConverterClient;
using Api.Infrastructure.Database.Context;
using Api.Infrastructure.Database.Entities;
using Api.Infrastructure.ObjectStorage;
using Api.Options;

namespace Api.Application.Features.Files.ConvertFile;

public class ConvertFileService : IConvertFileService
{
    private readonly FileConverterContext _dbContext;
    private readonly IFileValidator _fileValidator;
    private readonly IObjectStorage _objectStorage;
    private readonly IConverterClient _converterClient;
    private readonly ObjectStorageConfigOptions _minioDirectoriesOptions;
    
    public ConvertFileService(
        FileConverterContext dbContext,
        IFileValidator fileValidator,
        IObjectStorage objectStorage,
        IConverterClient converterClient,
        IConfiguration configuration
        )
    {
        _fileValidator = fileValidator;
        _dbContext = dbContext;
        _objectStorage = objectStorage;
        _converterClient = converterClient;
    }

    public async Task<ConvertFileResponse> ConvertFileAsync(ConvertFileRequest req, CancellationToken cancellation)
    {
        using var fileStream = req.OpenStream();
        var fileExtension = FileUtils.FileExtensionFromFileName(req.FileName);
        var targetExtension = req.TargetExtension;

        if (!_fileValidator.Validate(fileExtension, fileStream.Length))
        {
            throw new BadRequestException("Invalid file type or size");
        }

        var userId = await _dbContext.Users
            .Where(u => u.PublicId == req.CallerId)
            .Select(u => (int?)u.Id)
            .FirstOrDefaultAsync(cancellation)
            ?? throw new NotFoundException("User", req.CallerId.ToString());

        var fileContentType = FileUtils.FileExtensionToContentType(fileExtension);

        var originalGuid = Guid.CreateVersion7();
        var originalStorageKey = $"{userId}/{originalGuid}_{req.FileName}";

        var convertedGuid = Guid.CreateVersion7();
        var convertedFileName = $"{Path.GetFileNameWithoutExtension(req.FileName)}.{targetExtension}";
        var convertedStorageKey = $"{userId}/{convertedGuid}_{convertedFileName}";
        var convertedContentType = FileUtils.FileExtensionToContentType(targetExtension);

        await using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellation);
        try
        {
            var uploaded = await _objectStorage.UploadFileAsync(
                originalStorageKey, fileStream, fileStream.Length, fileContentType
            );

            if (!uploaded)
            {
                await transaction.RollbackAsync();
                throw new InternalServerErrorException("An uknown error has occured during file upload");
            }

            var sourceEntity = CreateFileDataEntity(
                userId, null, req.FileName, originalStorageKey, fileStream.Length, req.ContentType);

            _dbContext.Files.Add(sourceEntity);
            await _dbContext.SaveChangesAsync();

            var converted = await _converterClient.ConvertFileAsync(
                originalStorageKey, convertedStorageKey, req.TargetExtension);

            if (!converted)
            {
                await transaction.RollbackAsync();
                throw new InternalServerErrorException("An uknown error has occured during file convertion");
            }

            var convertedStream = await _objectStorage.GetOpenFileStreamAsync(convertedStorageKey);

            var outputEntity = CreateFileDataEntity(
                userId, sourceEntity.Id, convertedFileName, convertedStorageKey, convertedStream.Length, convertedContentType);

            _dbContext.Files.Add(outputEntity);
            await _dbContext.SaveChangesAsync(cancellation);

            await transaction.CommitAsync(cancellation);

            return new ConvertFileResponse
            {
                SourceFile = new Common.FileMetadata 
                { 
                    FileId = sourceEntity.PublicId,
                    FileName = sourceEntity.FileName,
                    ContentType = sourceEntity.ContentType,
                    FileSizeBytes = sourceEntity.FileSizeBytes,
                    Status = sourceEntity.Status,
                    CreatedAt = sourceEntity.CreatedAt,
                    DownloadUrl = $"/api/Files/{sourceEntity.PublicId}",
                    SourceFileDownloadUrl = null
                },
                ConvertedFile = new Common.FileMetadata 
                {
                    FileId = outputEntity.PublicId,
                    FileName = outputEntity.FileName,
                    ContentType = outputEntity.ContentType,
                    FileSizeBytes = outputEntity.FileSizeBytes,
                    Status = outputEntity.Status,
                    CreatedAt = outputEntity.CreatedAt,
                    DownloadUrl = $"/api/Files/{outputEntity.PublicId}",
                    SourceFileDownloadUrl = $"/api/Files/{sourceEntity.PublicId}"
                }
            };
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync(cancellation);
            throw;
        }
    }

    private static FileDataEntity CreateFileDataEntity(
        int ownerId, int? sourceFileId,
        string fileName, string storageKey,
        long sizeBytes, string contentType)
    {
        return new FileDataEntity
        {
            OwnerId = ownerId,
            PublicId = Guid.CreateVersion7(),
            SourceFileId = sourceFileId,
            FileName = fileName,
            StorageKey = storageKey,
            FileSizeBytes = sizeBytes,
            Status = "",
            ContentType = contentType,
        };
    }

}