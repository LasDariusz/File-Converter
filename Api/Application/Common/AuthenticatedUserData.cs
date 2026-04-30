namespace Api.Application.Common;

public class AuthenticatedUserData
{
    public required Guid UserId { get; set; }

    public required string Email { get; set; }

    public string? Username { get; set; }
}