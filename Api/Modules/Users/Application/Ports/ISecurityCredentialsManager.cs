using Api.Modules.Users.Domain.Models;

namespace Api.Modules.Users.Application.Ports;

public interface ISecurityCredentialsManager
{
    string HashPassword(string password);

    bool VerifyPassword(string password, UserModel user);

    RefreshTokenModel HashRefreshToken(RefreshTokenModel refreshToken);
}