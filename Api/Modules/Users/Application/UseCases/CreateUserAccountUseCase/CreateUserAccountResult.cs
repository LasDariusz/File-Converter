using Api.Modules.Users.Domain.Models;

namespace Api.Modules.Users.Application.UseCases.CreateUserAccountUseCase;

public record CreateUserAccountResult
{
    public required AccessTokenModel AccessToken { get; set; }

    public required RefreshTokenModel RefreshToken { get; set; }

    public required SafeUserDataModel SafeUserData { get; set; }
}