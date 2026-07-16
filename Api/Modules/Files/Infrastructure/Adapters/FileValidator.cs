using Api.Modules.Files.Application.Ports;
using Api.Modules.Files.Infrastructure.Config;
using Microsoft.Extensions.Options;


namespace Api.Modules.Files.Infrastructure.Adapters;

public class FileValidator : IFileValidator
{
    private readonly IOptions<UploadedFilesOptions> _fileOptions;

    public FileValidator(IOptions<UploadedFilesOptions> fileOptions)
    {
        _fileOptions = fileOptions;
    }


    public bool Validate(string extension, long sizeBytes)
    {
        var extensionLower = extension.ToLowerInvariant();

        var fileOptions = _fileOptions.Value;

        return fileOptions.AllowedImageFileFormats.Contains(extensionLower) &&
            sizeBytes < fileOptions.MaxImageFileSizeMb * 1_000_000;
    }

}