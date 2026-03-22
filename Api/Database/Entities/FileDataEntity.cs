using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Api.Database.Entities;

[Table("FileData")]
public class FileDataEntity
{
    [Key]
    [Column("ID")]
    public int Id { get; set; }

    [Required]
    public required Guid PublicId { get; set; }

    [Column("OwnerID")]
    public int OwnerId { get; set; }
    public UserEntity? Owner { get; set; }


    [Column("SourceFileID")]
    public int? SourceFileId { get; set; }
    public FileDataEntity? SourceFile { get; set; }
    public ICollection<FileDataEntity> DerivedFiles { get; set; } = new List<FileDataEntity>();


    [Required, MaxLength(255)]
    public required string FileName { get; set; }

    [Required, MaxLength(255)]
    public required string StorageKey { get; set; }

    [Required]
    public long FileSizeBytes { get; set; }

    [Required, MaxLength(100)]
    public required string Status { get; set; }

    [Required, MaxLength(100)]
    public required string ContentType { get; set; }

    [Required]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Required]
    public DateTime LastUpdatedAt { get; set; } = DateTime.UtcNow;

    [Required]
    public bool IsDeleted { get; set; } = false;
}
