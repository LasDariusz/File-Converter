using Api.Application.Common;

namespace Api.Infrastructure.Jwt;

public interface IJwtTokenGenerator
{
    string GenerateJwtToken(UserTokenData userTokenData);
}