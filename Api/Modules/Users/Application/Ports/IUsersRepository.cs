using Api.Modules.Users.Domain.Models;

namespace Api.Modules.Users.Application.Ports;

public interface IUsersRepository
{
    Task<bool> EmailExistsAsync(
        string email, 
        CancellationToken cancellationToken);

    Task<bool> SaveUserRefreshTokenAsync(
        UserModel user,
        RefreshTokenHashedModel refreshToken,
        CancellationToken cancellationToken); 

    Task<UserModel?> FindUserByEmailAsync(
    string Email,
    CancellationToken cancellationToken);

    Task<UserModel> CreateUserWithRefreshTokenAsync(
        UserModel user,
        RefreshTokenHashedModel refreshToken,
        CancellationToken cancellationToken);
}