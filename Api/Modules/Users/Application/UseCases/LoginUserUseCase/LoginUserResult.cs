using Api.Modules.Users.Domain.Models;

namespace Api.Modules.Users.Application.UseCases.LoginUserUseCase;

public record class LoginUserResult
{
    public required AccessTokenModel AccessToken { get; set; }

    public required RefreshTokenModel RefreshToken { get; set; }

    public required SafeUserDataModel SafeUserData { get; set; }
}