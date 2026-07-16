using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Api.Modules.Files.Infrastructure.Persistence.Entities;

[Table("Files")]
public class FileDataEntity
{
    [Key]
    public int FileId { get; set; }

    [Required]
    public Guid PublicId { get; set; }

    [Required]
    public int OwnerId { get; set; }

    [Required, MaxLength(255)]
    public required string FileName { get; set; }

    [Required, MaxLength(100)]
    public required string ContentType { get; set; }

    [Required, MaxLength(25)]
    public required string Extension { get; set; }

    [Required]
    public required long SizeBytes { get; set; }

    [Required, MaxLength(255)]
    public required string StorageKey { get; set; }

    public required DateTime CreatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }
}