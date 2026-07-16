using Api.Modules.Users.Application.Exceptions;
using Api.Modules.Users.Application.Ports;
using Api.Modules.Users.Application.UseCases.LoginUserUseCase;
using Api.Modules.Users.Domain.Models;
using Moq;

namespace ApiTests.Modules.Users.Application;

public class LoginUserUseCaseHandlerTests
{
    private readonly Mock<IUsersRepository> _usersRepositoryMock = new();
    private readonly Mock<ISecurityCredentialsManager> _securityCredentialsManagerMock = new();
    private readonly Mock<IJwtTokenGenerator> _jwtTokenGeneratorMock = new();

    private readonly LoginUserUseCaseHandler _system;

    public LoginUserUseCaseHandlerTests()
    {
        _system = new LoginUserUseCaseHandler(
            _usersRepositoryMock.Object,
            _securityCredentialsManagerMock.Object,
            _jwtTokenGeneratorMock.Object);
    }

    [Fact]
    public async Task ExecuteAsync_WhenValidCredentials_ShouldReturnTokensAndSafeUserData()
    {
        var command = new LoginUserCommand
        {
            Email = "example@email.com",
            Password = "Password123!"
        };

        var user = UserModel.Rehydrate(
            Guid.NewGuid(), 
            command.Email, 
            "stored-password-hash");

        var accessToken = new AccessTokenModel
        {
            Value = "access-token",
            ExpiresAt = DateTime.UtcNow.AddMinutes(30)
        };

        var refreshToken = new RefreshTokenModel
        {
            Value = "raw-refresh-token",
            ExpiresAt = DateTime.UtcNow.AddDays(5)
        };

        var refreshTokenHashed = new RefreshTokenHashedModel
        {
            Value = "hashed-refresh-token",
            ExpiresAt = refreshToken.ExpiresAt
        };

        _usersRepositoryMock
            .Setup(r => r.FindUserByEmailAsync(
                command.Email,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        _securityCredentialsManagerMock
            .Setup(m => m.VerifyPassword(command.Password, user))
            .Returns(true);

        _jwtTokenGeneratorMock
            .Setup(g => g.GenerateRefreshToken())
            .Returns(refreshToken);

        _securityCredentialsManagerMock
            .Setup(m => m.HashRefreshToken(refreshToken))
            .Returns(refreshTokenHashed);

       _usersRepositoryMock
            .Setup(r => r.SaveUserRefreshTokenAsync(
                user, 
                refreshTokenHashed,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        _jwtTokenGeneratorMock
            .Setup(g => g.GenerateJwtToken(user))
            .Returns(accessToken);



        var result = await _system.ExecuteAsync(
            command, 
            CancellationToken.None);

        Assert.Same(accessToken, result.AccessToken);
        Assert.Equal("access-token", result.AccessToken.Value);

        Assert.Same(refreshToken, result.RefreshToken);
        Assert.Equal("raw-refresh-token", result.RefreshToken.Value);

        Assert.Equal(user.PublicId, result.SafeUserData.UserId);
        Assert.Equal(user.Email, result.SafeUserData.Email);

        _usersRepositoryMock
            .Verify(r => r.SaveUserRefreshTokenAsync(
                user,
                refreshTokenHashed,
                It.IsAny<CancellationToken>()),
            Times.Once);
            
        _jwtTokenGeneratorMock
            .Verify(g => g.GenerateJwtToken(user), 
            Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_WhenUserDoesNotExist_ShouldThrowInvalidCredentialsException()
    {
        var command = new LoginUserCommand
        {
            Email = "missing@gmail.com",
            Password = "Password123!"
        };

        _usersRepositoryMock
            .Setup(r => r.FindUserByEmailAsync(
                command.Email,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((UserModel?)null);

        await Assert.ThrowsAsync<InvalidCredentialsException>(
            () => _system.ExecuteAsync(
                command,
                CancellationToken.None));

        _securityCredentialsManagerMock
            .Verify(m => m.VerifyPassword(
                It.IsAny<string>(), 
                It.IsAny<UserModel>()), 
            Times.Never);

        _jwtTokenGeneratorMock
            .Verify(g => g.GenerateRefreshToken(), Times.Never);

        _jwtTokenGeneratorMock
            .Verify(g => g.GenerateJwtToken(
                It.IsAny<UserModel>()), 
            Times.Never);
    }

    [Fact]
    public async Task ExecuteAsync_WhenInvalidPassword_ShouldThrowInvalidCredentialsException()
    {
        var command = new LoginUserCommand
        {
            Email = "example@email.com",
            Password = "InvalidPassword"
        };

        var user = UserModel.Rehydrate
            (Guid.NewGuid(),
            command.Email,
            "correct-password-hash");

        _usersRepositoryMock
            .Setup(r => r.FindUserByEmailAsync(
                command.Email,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        _securityCredentialsManagerMock
            .Setup(m => m.VerifyPassword(command.Password, user))
            .Returns(false);

        await Assert.ThrowsAsync<InvalidCredentialsException>(
            () => _system.ExecuteAsync(
                command,
                CancellationToken.None));

        _securityCredentialsManagerMock
            .Verify(m => m.VerifyPassword(
                command.Password, 
                user), 
            Times.Once);

        _jwtTokenGeneratorMock
            .Verify(g => g.GenerateRefreshToken(), Times.Never);

        _jwtTokenGeneratorMock
            .Verify(g => g.GenerateJwtToken(user), Times.Never);
    }

}