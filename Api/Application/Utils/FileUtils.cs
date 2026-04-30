namespace Api.Application.Utils;

public class FileUtils
{
    public static string FileExtensionToContentType(string fileExtension)
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

    public static string FileExtensionFromFileName(string fileName)
    {
        return Path.GetExtension(fileName)
            .TrimStart('.')
            .ToLowerInvariant();
    }

}