using Api.Modules.Users.Application.Ports;
using Api.Modules.Users.Domain.Models;
using System.Security.Cryptography;
using System.Text;

namespace Api.Modules.Users.Infrastructure.Adapters;

public class SecurityCredentialsManager : ISecurityCredentialsManager
{
    public string HashPassword(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password);
    }

    public RefreshTokenHashedModel HashRefreshToken(RefreshTokenModel refreshToken)
    {
        var bytes = Encoding.UTF8.GetBytes(refreshToken.Value);

        var hash = SHA256.HashData(bytes);

        return new RefreshTokenHashedModel
        {
            Value = Convert.ToHexString(hash),
            ExpiresAt = refreshToken.ExpiresAt

        };
    }

    public bool VerifyPassword(string password, UserModel user)
    {
        return BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);
    }

}