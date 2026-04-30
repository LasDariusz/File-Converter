using Api.Application.Validation;
using Api.Infrastructure.ConverterClient;
using Api.Infrastructure.Database.Context;
using Api.Infrastructure.Database.Entities;
using Api.Infrastructure.ObjectStorage;
using Api.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Api.Application.Exceptions;
using Api.Application.Utils;


namespace Api.Application.Features.Files.ConvertFile;

public class ConvertFileService : IConvertFileService
{
    private readonly FileConverterContext _dbContext;

    private readonly IFileValidator _fileValidator;

    private readonly IObjectStorage _objectStorage;
    private readonly IConverterClient _converterClient;

    private readonly MinioDirectoriesOptions _minioDirectoriesOptions;
    

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

    public async Task<ConvertFileResponse> ConvertFileAsync(ConvertFileRequest req)
    {
        using var fileStream = req.OpenStream();

        var fileExtension = FileUtils.FileExtensionFromFileName(req.FileName);

        if (!_fileValidator.Validate(fileExtension, fileStream.Length))
        {
            throw new BadRequestException();
        }

        var userId = await _dbContext.Users
            .Where(u => u.PublicId == req.CallerId)
            .Select(u => (int?)u.Id)
            .FirstOrDefaultAsync();

        if (userId is null)
        {
            throw new NotFoundException("User", req.CallerId.ToString());
        }

        var originalFileGuid = Guid.CreateVersion7();
        var originalFileStorageKey = $"{userId}/{originalFileGuid}_{req.FileName}";

        var convertedFileGuid = Guid.CreateVersion7();
        var convertedFileName = $"{Path.GetFileNameWithoutExtension(req.FileName)}.{req.TargetExtension}";
        var convertedFileStorageKey = $"{userId}/{convertedFileGuid}_{convertedFileName}";

        var fileContentType = FileUtils.FileExtensionToContentType(fileExtension);


        await using var transaction = await _dbContext.Database.BeginTransactionAsync();
        try
        {
            var uploaded = await _objectStorage.UploadFileAsync(
                originalFileStorageKey, fileStream, fileStream.Length, fileContentType
            );

            if (!uploaded)
            {
                await transaction.RollbackAsync();
                throw new Exception();
            }

            var sourceEntity = CreateFileDataEntity(
                userId!.Value, null,
                req.FileName, originalFileStorageKey,
                fileStream.Length, req.ContentType);

            _dbContext.Files.Add(sourceEntity);
            await _dbContext.SaveChangesAsync();

            var converted = await _converterClient.ConvertFileAsync(
                originalFileStorageKey, convertedFileStorageKey, req.TargetExtension);

            if (!converted)
            {
                await transaction.RollbackAsync();
                throw new Exception();
            }

            var convertedStream = await _objectStorage.GetOpenFileStreamAsync(convertedFileStorageKey);

            var outputEntity = CreateFileDataEntity(
                userId!.Value, sourceEntity.Id,
                convertedFileName, convertedFileStorageKey,
                convertedStream.Length, fileContentType);

            _dbContext.Files.Add(outputEntity);
            await _dbContext.SaveChangesAsync();
            await transaction.CommitAsync();

            return new ConvertFileResponse
            {
                FileId = convertedFileGuid,
                ContentType = fileContentType,
                FileName = convertedFileName,
                SizeBytes = convertedStream.Length,
                DownloadUrl = null
            };
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
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