namespace Api.Modules.Users.Domain.Models;

public class AccessTokenModel
{
    public required string Value { get; set; }

    public required int ExpiresInMinutes { get; set; }
}