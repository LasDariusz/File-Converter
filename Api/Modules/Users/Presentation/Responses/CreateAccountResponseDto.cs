namespace Api.Modules.Users.Presentation.Responses;

public record class CreateAccountResponseDto
{
    public required string AccessToken { get; init; }

    public DateTime AccessTokenExpiresAt { get; init; }

    public required string RefreshToken { get; init; }

    public DateTime RefreshTokenExpiresAt { get; init; }

    public required Guid UserId { get; init; }

    public required string Email { get; init; }
}