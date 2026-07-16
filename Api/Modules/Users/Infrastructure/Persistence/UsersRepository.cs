using Api.Modules.Users.Application.Ports;
using Api.Modules.Users.Domain.Models;
using Api.Modules.Users.Infrastructure.Persistence.Entities;
using Api.Shared.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Api.Modules.Users.Infrastructure.Persistence;

public class UsersRepository : IUsersRepository
{
    private readonly FileConverterContext _dbContext;

    public UsersRepository(FileConverterContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<UserModel> CreateUserWithRefreshTokenAsync(
        UserModel user, 
        RefreshTokenModel refreshToken, 
        CancellationToken cancellationToken)
    {
        var userEntity = new UserEntity
        {
            PublicId = user.PublicId,
            Email = user.Email,
            PasswordHash = user.PasswordHash,
            CreatedAt = DateTime.UtcNow
        };

        await _dbContext.Users.AddAsync(userEntity, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return UserModel.Rehydrate(
            userEntity.PublicId, 
            userEntity.Email, 
            userEntity.PasswordHash);
    }

    public async Task<bool> EmailExistsAsync(
        string email, 
        CancellationToken cancellationToken)
    {
        return await _dbContext.Users
            .AnyAsync(u => u.Email == email, cancellationToken);
    }

    public async Task<UserModel?> FindUserByEmailAsync(
        string email, 
        CancellationToken cancellationToken)
    {
        var user = await _dbContext.Users
            .SingleOrDefaultAsync(u => u.Email == email, cancellationToken);

        return user is null 
            ? null 
            : UserModel.Rehydrate(user.PublicId, user.Email, user.PasswordHash);
    }

    public Task<bool> SaveUserRefreshTokenAsync(
        UserModel user, 
        RefreshTokenModel refreshToken, 
        CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}