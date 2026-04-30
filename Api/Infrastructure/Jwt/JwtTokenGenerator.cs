using Api.Application.Common;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Api.Infrastructure.Jwt;

public class JwtTokenGenerator : IJwtTokenGenerator
{
    private readonly string _jwtSecret;

    public JwtTokenGenerator()
    {
        _jwtSecret = "SuperExtraSecretJwtKey1234567890";
    }

    public string GenerateJwtToken(UserTokenData userTokenData)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, userTokenData.UserId.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, userTokenData.Email),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSecret));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: "xyz",
            audience: "xyz",
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(30),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler()
            .WriteToken(token);
    }

}