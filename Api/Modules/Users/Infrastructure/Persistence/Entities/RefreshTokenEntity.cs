using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Api.Modules.Users.Infrastructure.Persistence.Entities;

[Table("Tokens")]
public class RefreshTokenEntity
{
    [Key]
    public long RefreshTokenId { get; set; }

    [Required]
    public int UserId { get; set; }

    [Required, MaxLength(64)]
    public required string TokenHash { get; set; }

    [Required]
    public DateTime ExpiresAt { get; set; }

    [Required]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? RevokedAt { get; set; }

    [MaxLength(64)]
    public string? ReplacedByTokenHash { get; set; }
}