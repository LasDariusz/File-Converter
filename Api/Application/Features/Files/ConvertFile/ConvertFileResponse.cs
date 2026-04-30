namespace Api.Application.Features.Files.ConvertFile;

public class ConvertFileResponse
{
    public required Guid FileId { get; set; }

    public required string ContentType { get; set; }

    public required string FileName { get; set; }

    public required long SizeBytes { get; set; }

    public string? DownloadUrl { get; set; }
}