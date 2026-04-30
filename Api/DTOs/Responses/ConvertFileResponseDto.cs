namespace Api.DTOs.Responses;

public class ConvertFileResponseDto
{
    public required string FileId { get; set; }

    public required string ContentType { get; set; }

    public required string FileName { get; set; }

    public required long SizeByets { get; set; }

    public string? DownloadUrl { get; set; }
}