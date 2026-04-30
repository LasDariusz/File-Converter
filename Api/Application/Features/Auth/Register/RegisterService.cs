using Microsoft.EntityFrameworkCore;

using Api.Infrastructure.Database.Context;
using Api.Infrastructure.Database.Entities;
using Api.Infrastructure.Jwt;

using Api.Application.Exceptions;
using Api.Application.Common;

namespace Api.Application.Features.Auth.Register;

public class RegisterService : IRegisterService
{
    private readonly FileConverterContext _fileConverterContext;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;

    public RegisterService(
        FileConverterContext fileConverterContext,
        IJwtTokenGenerator jwtTokenGenerator
    )
    {
        _fileConverterContext = fileConverterContext;
        _jwtTokenGenerator = jwtTokenGenerator;
    }

    public async Task<RegisterResponse> RegisterAsync(RegisterRequest req)
    {
        var email = req.Email;
        var password = req.Password;

        var userExists = await _fileConverterContext.Users
            .AnyAsync(u => u.Email == email);

        if (userExists)
        {
            throw new ConflictException("User", email);
        }

        await using var transaction = await _fileConverterContext.Database
            .BeginTransactionAsync();
        try
        {
            string passwordHash = BCrypt.Net.BCrypt.HashPassword(password);

            var user = new UserEntity
            {
                PublicId = Guid.CreateVersion7(),
                Email = email,
                Username = "",
                PasswordHash = passwordHash
            };

            await _fileConverterContext.Users.AddAsync(user);
            await _fileConverterContext.SaveChangesAsync();
            await transaction.CommitAsync();

            var token = _jwtTokenGenerator.GenerateJwtToken(new UserTokenData 
            { 
                UserId = user.PublicId,
                Email = email
            });

            return new RegisterResponse
            {
                AccessToken = token,
                RefreshToken = null,
                ExpiresInMinutes = null,
                UserData = new AuthenticatedUserData
                {
                    UserId = user.PublicId,
                    Email = email,
                    Username = user.Username
                }
            };
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

}