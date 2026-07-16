using Api.Application.Common;
using Api.Modules.Users.Application.Ports;
using Api.Modules.Users.Domain.Models;
using Api.Modules.Users.Infrastructure.Config;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Api.Modules.Users.Infrastructure.Adapters;

public class JwtTokenGenerator : IJwtTokenGenerator
{
    private readonly JwtConfigOptions _jwtConfigOptions;

    public JwtTokenGenerator(IOptions<JwtConfigOptions> jwtConfigOptions)
    {
        _jwtConfigOptions = jwtConfigOptions.Value;
    }

    public GeneratedAccessToken GenerateJwtToken(UserTokenData userTokenData)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, userTokenData.UserId.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, userTokenData.Email),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var expiresAt = DateTime.UtcNow
            .AddMinutes(_jwtConfigOptions.AccessTokenValidMinutes);

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtConfigOptions.Secret));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _jwtConfigOptions.Issuer,
            audience: _jwtConfigOptions.Audience,
            claims: claims,
            expires: expiresAt,
            signingCredentials: creds
        );

        return new GeneratedAccessToken
        {
            AccessToken = new JwtSecurityTokenHandler().WriteToken(token),
            ExpiresAt = expiresAt
        };
    }

    public AccessTokenModel GenerateJwtToken(UserModel user)
    {
        throw new NotImplementedException();
    }

    public RefreshTokenModel GenerateRefreshToken(UserModel user)
    {
        throw new NotImplementedException();
    }

    public string GenereteRefreshToken()
    {
        return Convert.ToBase64String(
            RandomNumberGenerator.GetBytes(64)
        );
    }

}