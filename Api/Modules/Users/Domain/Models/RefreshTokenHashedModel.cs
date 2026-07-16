namespace Api.Modules.Users.Domain.Models;

public class RefreshTokenHashedModel
{
    public required string Value { get; init; }

    public DateTime ExpiresAt { get; init; }
}