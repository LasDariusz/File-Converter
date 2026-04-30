namespace Api.DTOs.Shared;

public class AuthenticatedUserDataDto
{
    public required Guid UserId { get; set; }

    public required string Email { get; set; }

    public string? Username { get; set; }
}