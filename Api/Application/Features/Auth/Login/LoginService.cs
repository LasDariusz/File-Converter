using Microsoft.EntityFrameworkCore;

using Api.Application.Exceptions;
using Api.Application.Common;

using Api.Infrastructure.Database.Context;
using Api.Infrastructure.Jwt;


namespace Api.Application.Features.Auth.Login;

public class LoginService : ILoginService
{
    private readonly FileConverterContext _fileConverterContext;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;

    public LoginService(
        FileConverterContext fileConverterContext, 
        IJwtTokenGenerator jwtTokenGenerator
    )
    {
        _fileConverterContext = fileConverterContext;
        _jwtTokenGenerator = jwtTokenGenerator;
    }

    public async Task<LoginResponse> LoginAsync(LoginRequest req)
    {
        var email = req.Email;
        var password = req.Password;

        var userEntity = await _fileConverterContext.Users
            .FirstOrDefaultAsync(u => u.Email == email);

        if (userEntity == null)
        {
            throw new NotFoundException("User", email);
        }

        bool validPassword = BCrypt.Net.BCrypt
            .Verify(password, userEntity.PasswordHash);

        if (!validPassword)
        {
            throw new UnauthorizedException("Invalid password");
        }

        var token = _jwtTokenGenerator.GenerateJwtToken(new UserTokenData
        { 
            UserId = userEntity.PublicId,
            Email = email
        });

        var userData = new AuthenticatedUserData
        {
            UserId = userEntity.PublicId,
            Email = userEntity.Email,
            Username = userEntity.Username,
        };

        return new LoginResponse
        {
            AccessToken = token,
            RefreshToken = null,
            ExpiresInMinutes = null,
            UserData = userData
        };
    }

}