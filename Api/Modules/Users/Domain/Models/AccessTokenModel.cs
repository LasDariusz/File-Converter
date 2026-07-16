namespace Api.Modules.Users.Domain.Models;

public class AccessTokenModel
{
    public required string Value { get; init; }

    public DateTime ExpiresAt { get; init; }
}