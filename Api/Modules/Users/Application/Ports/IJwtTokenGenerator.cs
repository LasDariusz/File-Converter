using Api.Modules.Users.Domain.Models;

namespace Api.Modules.Users.Application.Ports;

public interface IJwtTokenGenerator
{
    AccessTokenModel GenerateJwtToken(UserModel user);

    RefreshTokenModel GenerateRefreshToken(UserModel user);
}