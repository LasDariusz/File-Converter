namespace Api.Modules.Users.Domain.Models;

public class SafeUserDataModel
{
    public required Guid UserId { get; set; }

    public required string Email { get; set; }
}