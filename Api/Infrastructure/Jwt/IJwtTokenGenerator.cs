using Api.Application.Common;

namespace Api.Infrastructure.Jwt;

public interface IJwtTokenGenerator
{
    GeneratedAccessToken GenerateJwtToken(UserTokenData userTokenData);

    string GenereteRefreshToken();
}