namespace Api.Application.Common;

public class UserTokenData
{
    public required Guid UserId { get; set; }

    public required string Email { get; set; }
}