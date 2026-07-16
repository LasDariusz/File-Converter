using Api.Modules.Files.Domain.Models.Conversions;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Api.Modules.Files.Infrastructure.Persistence.Entities;

[Table("Conversions")]
public class ConversionJobEntity
{
    [Key]
    public int ConversionId { get; set; }

    [Required]
    public Guid PublicId { get; set; } = Guid.CreateVersion7();

    [Required]
    public int OwnerId { get; set; }

    [Required]
    public int SourceFileId { get; set; }

    public int? OutputFileId { get; set; }

    [Required, MaxLength(25)]
    public required string SourceFormat {  get; set; }

    [Required, MaxLength(25)]
    public required string OutputFormat { get; set; }

    [Required]
    public required ConversionStatusModel Status { get; set; } = ConversionStatusModel.Queued;

    [MaxLength(255)]
    public string? ErrorMessage { get; set; }

    public int AttemptCount { get; set; }

    public required DateTime CreatedAt { get; set; }

    public DateTime? StartedAt { get; set; }

    public DateTime? FinishedAt { get; set; }
}