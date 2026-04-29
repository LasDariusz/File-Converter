using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

using Api.Database.Context;
using Api.Database.Entities;
using Api.DomainModels.GenericResult;
using Api.DomainModels.Features.Register;
using Api.DomainModels.Features.RefreshToken;
using Api.DomainModels.Features.Login;
using Api.Exceptions;

namespace Api.Services.Auth;

public class AuthService : IAuthService
{
    private readonly FileConverterContext _dbContext;
    private readonly string _jwtSecret;

    public AuthService(FileConverterContext fileConverterContext)
    {
        _dbContext = fileConverterContext;
        _jwtSecret = "SuperExtraSecretJwtKey1234567890";
    }

    private string GenerateJwtToken(UserEntity user)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.PublicId.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
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

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public async Task<Result<LoginModelResponse>> LoginAsync(LoginModel model)
    {
        var email = model.Email;
        var password = model.Password;

        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
        {
            return Result<LoginModelResponse>.Failure("Empty login credentials", ErrorType.Validation);
        }

        var userEntity = await _dbContext.Users
            .FirstOrDefaultAsync(u => u.Email == email);

        if (userEntity == null)
        {
            throw new UnauthorizedException();
            // return Result<LoginModelResponse>.Failure($"User '{email}' not found", ErrorType.Validation);
        }

        bool validPassword = BCrypt.Net.BCrypt
            .Verify(password, userEntity.PasswordHash);

        if (!validPassword)
        {
            throw new UnauthorizedException();
            // return Result<LoginModelResponse>.Failure("Wrong password", ErrorType.Validation);
        }

        var token = GenerateJwtToken(userEntity);

        return Result<LoginModelResponse>.Success(new LoginModelResponse { 
            PublicId = userEntity.PublicId,
            Username = userEntity.Username,
            Email = userEntity.Email,
            Token = token 
        });
    }

    public async Task<Result<RegisterModelResponse>> RegisterAsync(RegisterModel model)
    {
        var email = model.Email;
        var username = model.Username;
        var password = model.Password;

        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
        {
            return Result<RegisterModelResponse>.Failure("Empty registration credentials", ErrorType.Validation);
        }

        var userExists = await _dbContext.Users
            .AnyAsync(u => u.Username == username || u.Email == email);

        if (userExists)
        {
            //return Result<RegisterModelResponse>.Failure($"User '{username}' '{email} already exists", ErrorType.Conflict);
        }

        await using var transaction = await _dbContext.Database.BeginTransactionAsync();
        try
        {
            string passwordHash = BCrypt.Net.BCrypt.HashPassword(password);

            var user = new UserEntity
            {
                PublicId = Guid.CreateVersion7(),
                Email = email,
                Username = username,
                PasswordHash = passwordHash
            };

            await _dbContext.Users.AddAsync(user);
            await _dbContext.SaveChangesAsync();
            await transaction.CommitAsync();

            var token = GenerateJwtToken(user);

            return Result<RegisterModelResponse>.Success(new RegisterModelResponse
            {
                PublicId = user.PublicId,
                Username = user.Username,
                Email = user.Email,
                Token = token
            });
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            return Result<RegisterModelResponse>.Failure("Unknown error", ErrorType.Unknown);
        }
    }

    public Task<Result<RefreshTokenModelResponse>> RefreshTokenAsync(RefreshTokenModel model)
    {
        throw new NotImplementedException();
    }

}