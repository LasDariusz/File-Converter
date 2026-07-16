using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Api.Modules.Users.Infrastructure.Persistence.Entities;

[Table("Users")]
public class UserEntity
{
    [Key]
    public int UserId { get; set; }

    [Required, MaxLength(128)]
    public Guid PublicId { get; set; }

    [Required, MaxLength(255)]
    public required string Email { get; set; }

    [Required, MaxLength(255)]
    public required string PasswordHash { get; set; }

    public required DateTime CreatedAt { get; set; }
}