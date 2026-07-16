using Api.Modules.Users.Application.Ports;
using Api.Modules.Users.Application.UseCases.CreateUserAccountUseCase;
using Moq;

namespace ApiTests.Modules.Users.Application;

public class CreateUserAccountUseCaseHandlerTests
{
    private readonly Mock<IUsersRepository> _usersRepositoryMock = new();
    private readonly Mock<ISecurityCredentialsManager> _securityCredentialsManagerMock = new();
    private readonly Mock<IJwtTokenGenerator> _jwtTokenGeneratorMock = new();

    private readonly CreateUserAccountUseCaseHandler _system;

    public CreateUserAccountUseCaseHandlerTests()
    {
        _system = new CreateUserAccountUseCaseHandler(
            _usersRepositoryMock.Object,
            _securityCredentialsManagerMock.Object,
            _jwtTokenGeneratorMock.Object);
    }


}