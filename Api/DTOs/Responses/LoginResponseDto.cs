using Api.DTOs.Shared;


namespace Api.DTOs.Responses;

public class LoginResponseDto
{
    public required string AccessToken { get; set; }

    public int? ExpiresInMinutes { get; set; }

    public string? RefreshToken { get; set; }

    public required AuthenticatedUserDataDto UserData { get; set; }
}