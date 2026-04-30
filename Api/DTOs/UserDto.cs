using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Api.DTOs;

public class UserDto
{
    public int Id { get; set; }

    [Required, MaxLength(255), EmailAddress]
    [Column("email")]
    public string Email { get; set; }

    [Required, MaxLength(255)]
    [Column("username")]
    public string Username { get; set; }

    [Required, MaxLength(255)]
    [Column("passwordHash")]
    public string PasswordHash { get; set; }
}
