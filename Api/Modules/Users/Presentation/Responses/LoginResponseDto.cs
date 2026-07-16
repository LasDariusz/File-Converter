namespace Api.Modules.Users.Presentation.Responses;

public class LoginResponseDto
{
    public required string AccessToken { get; set; }

    public required int AccessTokenExpiresInMinutes { get; set; }

    public required string RefreshToken { get; set; }

    public required int RefreshTokenExpiresInHours { get; set; }

    public required Guid UserId { get; set; }

    public required string Email { get; set; }
}