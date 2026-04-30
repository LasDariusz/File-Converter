namespace Api.DTOs.Responses;

public class FetchUserFilesMetadataResponseDto
{
    public class FileMetadataDto
    {
        public Guid fileId { get; set; }

        public string FileName { get; set; }

        public string ContentType { get; set; }

        public long FileSizeBytes { get; set; }

        public string Status { get; set; }

        public DateTime CreatedAt { get; set; }

        public string DownloadUrl { get; set; }

        public string? SourceFileDownloadUrl { get; set; }
    }

    public required IEnumerable<FileMetadataDto> UserFilesMetadata { get; set; }
}