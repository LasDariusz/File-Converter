using Api.Modules.Files.Domain.Models.Files.Exceptions;

namespace Api.Modules.Files.Domain.Models.Files;

public class FileFormat
{
    private static readonly IReadOnlyDictionary<string, string> ContentTypes =
        new Dictionary<string, string>
        {
            ["jpeg"] = "image/jpeg",
            ["jpg"] = "image/jpeg",
            ["png"] = "image/png",
            ["webp"] = "image/webp",
            ["ppm"] = "image/x-portable-pixmap",
            ["bmp"] = "image/bmp",
            ["svg"] = "image/svg+xml",
            ["heic"] = "image/heic",
            ["heif"] = "image/heif",

            ["mp4"] = "video/mp4",
            ["webm"] = "video/webm",
            ["mov"] = "video/quicktime",

            ["mp3"] = "audio/mpeg",
            ["wav"] = "audio/wav",
            ["ogg"] = "audio/ogg",
            ["flac"] = "audio/flac",
            ["aac"] = "audio/aac",

            ["pdf"] = "application/pdf",
            ["docx"] =
                "application/vnd.openxmlformats-officedocument.wordprocessingml.document"
        };

    public string Extension { get; }

    public string ContentType { get; }

    private FileFormat(string extension, string contentType) 
    { 
        Extension = extension;
        ContentType = contentType;
    }

    public static FileFormat FromFileName(string fileName)
    {
        var extension = Path.GetExtension(fileName)
            .TrimStart('.');

        return FromExtension(extension);
    }

    public static FileFormat FromExtension(string extension)
    {
        var extensionLower = extension
            .Trim()
            .TrimStart('.')
            .ToLowerInvariant();

        if (!ContentTypes.TryGetValue(extensionLower, out var contentType))
            throw new InvalidFileException();

        return new FileFormat(extensionLower, contentType);
    }

    public override string ToString()
    {
        return Extension;
    }

}