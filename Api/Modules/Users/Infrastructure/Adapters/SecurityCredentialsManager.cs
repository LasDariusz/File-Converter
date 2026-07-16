using Api.Modules.Users.Application.Ports;
using Api.Modules.Users.Domain.Models;

namespace Api.Modules.Users.Infrastructure.Adapters;

public class SecurityCredentialsManager : ISecurityCredentialsManager
{
    public string HashPassword(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password);
    }

    public RefreshTokenModel HashRefreshToken(RefreshTokenModel refreshToken)
    {
        throw new NotImplementedException();
    }

    public bool VerifyPassword(string password, UserModel user)
    {
        return BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);
    }

}