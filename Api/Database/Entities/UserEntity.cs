using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Api.Database.Entities;

[Table("_User")]
public class UserEntity
{
    [Key]
    [Column("ID")]
    public int Id { get; set; }

    [Required]
    public required Guid PublicId { get; set; }

    [Required, MaxLength(255), EmailAddress]
    public required string Email { get; set; }

    [Required, MaxLength(255)]
    public required string Username {  get; set; }

    [Required, MaxLength(255)]
    public required string PasswordHash { get; set; }

    public ICollection<FileDataEntity> UserFiles { get; set; } = new List<FileDataEntity>();
}
