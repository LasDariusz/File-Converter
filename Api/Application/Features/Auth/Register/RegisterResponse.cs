using Api.Application.Common;

namespace Api.Application.Features.Auth.Register;

public class RegisterResponse
{
    public required string AccessToken { get; set; }

    public int? ExpiresInMinutes { get; set; }

    public string? RefreshToken { get; set; }

    public required AuthenticatedUserData UserData { get; set; }
}