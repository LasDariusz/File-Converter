using Api.Options;
using Api.Services.ConverterClient;
using Api.Services.ObjectStorage;
using Api.Database.Context;
using Api.Database.Entities;
using Api.DomainModels.GenericResult;
using Api.DomainModels.Features.ConvertFile;
using Api.DomainModels.Features.DeleteFile;
using Api.DomainModels.Features.UploadFile;
using Api.DomainModels.Features.FetchFile;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;


namespace Api.Services.Files;

public class FilesService : IFilesService
{
    private readonly FileConverterContext _dbContext;

    private readonly IObjectStorage _objectStorage;
    private readonly IConverterClient _converterClient;

    private readonly MinioDirectoriesOptions _minioDirectoriesOptions;
    private readonly Options.FileOptions _fileOptions;

    public FilesService(
        FileConverterContext fileConverterContext,
        IObjectStorage objectStorage,
        IConverterClient converterClient,
        IConfiguration configuration,
        IOptions<Options.FileOptions> fileOptions)
    {
        _dbContext = fileConverterContext;
        _objectStorage = objectStorage;
        _converterClient = converterClient;
        _fileOptions = fileOptions.Value;
    }


    private async Task<int?> GetUserIdAsync(Guid userPublicId)
    {
        var userId = await _dbContext.Users
            .Where(u => u.PublicId == userPublicId)
            .Select(u => (int?)u.Id)
            .FirstOrDefaultAsync();

        return userId;
    }

    private string? CheckImageFileSizeLimit(string fileExtension, long sizeBytes)
    {
        var fileExtensionLower = fileExtension.ToLowerInvariant();

        if (!_fileOptions.AllowedImageFileFormats.Contains(fileExtensionLower))
            return $"Format '{fileExtensionLower}' is not allowed";

        if (sizeBytes > _fileOptions.MaxImageFileSizeMb * 1_000_000)
            return $"File is too large (max {_fileOptions.MaxImageFileSizeMb} MB)";

        return null;
    }

    private static string FileExtensionToContentType(string fileExtension)
    {
        return fileExtension.ToLowerInvariant() switch
        {
            "jpeg" or "jpg" => "image/jpeg",
            "png" => "image/png",
            "webp" => "image/webp",
            "ppm" => "image/x-portable-pixmap",
            "bmp" => "image/bmp",
            "svg+xml" => "image/svg+xml",
            "mp4" => "video/mp4",
            "webm" => "video/webm",
            "mov" => "video/quicktime",
            "mp3" => "audio/mpeg",
            "wav" => "audio/wav",
            "ogg" => "audio/ogg",
            "flac" => "audio/flac",
            "aac" => "audio/aac",
            "pdf" => "application/pdf",
            "docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
            "heic" or "heif" => "image/heic",
            _ => "application/octet-stream"
        }; 
    }

    private static string FileExtensionFromFileName(string fileName)
    {
        return Path.GetExtension(fileName)
            .TrimStart('.')
            .ToLowerInvariant();
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


    public async Task<Result<FetchFileModelResponse>> GetFileByGuidAsync(FetchFileModel model)
    {
        var fileInfo = await _dbContext.Files
            .Where(f => f.PublicId == model.FileId)
            .Select(f => new
            {
                f.StorageKey,
                f.FileName,
                f.ContentType,
                OwnerPublicId = f.Owner!.PublicId
            })
            .FirstOrDefaultAsync();

        if (fileInfo is null)
            return Result<FetchFileModelResponse>
                .Failure($"File {model.FileId} not found", ErrorType.NotFound);

        if (fileInfo.OwnerPublicId != model.UserId)
            return Result<FetchFileModelResponse>
                .Failure("No access", ErrorType.Unauthorized);

        var stream = await _objectStorage.GetOpenFileStreamAsync(fileInfo.StorageKey);

        if (stream is null)
            return Result<FetchFileModelResponse>
                .Failure($"File {model.FileId} not found in storage", ErrorType.NotFound);

        return Result<FetchFileModelResponse>.Success(new FetchFileModelResponse
        {
            FileStream = stream,
            ContentType = fileInfo.ContentType,
            FileName = fileInfo.FileName
        });
    }

    public async Task<Result<UploadFileModelResponse>> UploadFileAsync(UploadFileModel model)
    {
        throw new NotImplementedException();
    }

    public async Task<Result<ConvertFileModelResponse>> ConvertFileToSpecifiedFormatAsync(ConvertFileModel model)
    {
        using var fileStream = model.OpenStream();

        var sourceExtensionLower = FileExtensionFromFileName(model.FileName);
        var targetExtensionLower = model.TargetExtension.TrimStart('.').ToLowerInvariant();

        var sizeError = CheckImageFileSizeLimit(sourceExtensionLower, fileStream.Length);

        if (sizeError is not null)
            return Result<ConvertFileModelResponse>.Failure(sizeError, ErrorType.BUSINESS_LOGIC);

        var userId = await GetUserIdAsync(model.UserId);
        if (userId is null)
            return Result<ConvertFileModelResponse>.Failure(
                $"User {model.UserId} not found", ErrorType.NotFound);

        var sourceGuid = Guid.CreateVersion7();
        var storageKey = $"{model.UserId}/{sourceGuid}_{model.FileName}";

        var destinationGuid = Guid.CreateVersion7();
        var destinationName = $"{Path.GetFileNameWithoutExtension(model.FileName)}.{targetExtensionLower}";
        var outputKey = $"{model.UserId}/{destinationGuid}_{destinationName}";

        var outputContentType = FileExtensionToContentType(targetExtensionLower);


        await using var transaction = await _dbContext.Database.BeginTransactionAsync();
        try
        {
            var uploaded = await _objectStorage.UploadFileAsync(
                storageKey, fileStream, fileStream.Length, model.ContentType);

            if (!uploaded)
            {
                await transaction.RollbackAsync();
                return Result<ConvertFileModelResponse>.Failure(
                    "File upload failed", ErrorType.Unknown);
            }

            var sourceEntity = CreateFileDataEntity(
                userId.Value, null,
                model.FileName, storageKey,
                fileStream.Length, model.ContentType);

            _dbContext.Files.Add(sourceEntity);
            await _dbContext.SaveChangesAsync();

            var converted = await _converterClient.ConvertFileAsync(
                storageKey, outputKey, targetExtensionLower);

            if (!converted)
            {
                await transaction.RollbackAsync();
                return Result<ConvertFileModelResponse>.Failure(
                    "File conversion failed", ErrorType.Unknown);
            }

            var convertedStream = await _objectStorage.GetOpenFileStreamAsync(outputKey);

            var outputEntity = CreateFileDataEntity(
                userId.Value, sourceEntity.Id,
                destinationName, outputKey,
                convertedStream.Length, outputContentType);

            _dbContext.Files.Add(outputEntity);
            await _dbContext.SaveChangesAsync();
            await transaction.CommitAsync();

            return Result<ConvertFileModelResponse>.Success(new ConvertFileModelResponse
            {
                OutputStream = convertedStream,
                ContentType = outputContentType,
            });
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            return Result<ConvertFileModelResponse>.Failure(ErrorType.Unknown);
        }
    }

    public async Task<Result<DeleteFileModelResponse>> DeleteFileByGuidAsync(DeleteFileModel model)
    {
        throw new NotImplementedException();
    }

}