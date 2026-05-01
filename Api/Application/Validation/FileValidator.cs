using Microsoft.Extensions.Options;


namespace Api.Application.Validation;

public class FileValidator : IFileValidator
{
    private readonly IOptions<Options.UploadedFilesOptions> _fileOptions;

    public FileValidator(IOptions<Options.UploadedFilesOptions> fileOptions)
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