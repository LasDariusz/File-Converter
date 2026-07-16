using System.ComponentModel.DataAnnotations;

namespace Api.Modules.Users.Presentation.Requests;

public class LoginRequestDto
{
    [Required, MaxLength(255), EmailAddress]
    public required string Email {  get; set; }

    [Required, MaxLength(255)]
    public required string Password { get; set; }
}