using Microsoft.Extensions.Options;
using System.ComponentModel.DataAnnotations;

namespace Api.DTOs.ValidationAttributes;

public class AllowedFormats : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        var settings = validationContext
            .GetService(typeof(IOptions<Options.UploadedFilesOptions>)) as IOptions<Options.UploadedFilesOptions>;

        if (settings is null)
            throw new InvalidOperationException("FileOptions not registered in DI");

        var allowed = settings.Value.AllowedImageFileFormats
            .Select(f => f.ToLowerInvariant())
            .ToHashSet();

        var ext = value?.ToString()?.ToLowerInvariant();

        if (string.IsNullOrEmpty(ext) || !allowed.Contains(ext))
        {
            var allowedStr = string.Join(", ", allowed.Order());
            return new ValidationResult(
                $"Format '{ext}' is not allowed. Supported formats: {allowedStr}");
        }

        return ValidationResult.Success;
    }
}