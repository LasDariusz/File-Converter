using System.ComponentModel.DataAnnotations;

namespace Api.DTOs.Features.Login;

public class LoginRequestDto
{
    [EmailAddress]
    public required string Email {  get; set; }

    [MaxLength(255), MinLength(8)]
    public required string Password { get; set; }
}