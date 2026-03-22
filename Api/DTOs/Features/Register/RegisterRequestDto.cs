using System.ComponentModel.DataAnnotations;

namespace Api.DTOs.Features.Register;

public class RegisterRequestDto
{
    [EmailAddress]
    public required string Email { get; set; }

    [Required, MaxLength(255)]
    public required string Username { get; set; }

    [Required, MaxLength(255)]
    public required string Password { get; set; }

}