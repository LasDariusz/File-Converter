using Backend.Application.Features.Auth.Login;
using Backend.Infrastructure.Jwt;
using Moq;

namespace ApiTests.Application.Features.Auth;

public class LoginServiceTests
{
    private readonly Mock<IJwtTokenGenerator> _jwtTokenGeneratorMock = new();

    private readonly LoginService _sut;

    public LoginServiceTests()
    {
        _sut = new LoginService(
            fileConverterContext: ,
            jwtTokenGenerator: _jwtTokenGeneratorMock.Object
        );
    }

    [Fact]
    public async Task LoginAsync_WhenCorrectLoginRequest_ShouldReturnLoginResponse()
    {

    }

}