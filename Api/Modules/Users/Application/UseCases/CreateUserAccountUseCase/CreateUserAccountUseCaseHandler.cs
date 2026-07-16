using Api.Modules.Users.Application.Exceptions;
using Api.Modules.Users.Application.Ports;
using Api.Modules.Users.Domain.Models;

namespace Api.Modules.Users.Application.UseCases.CreateUserAccountUseCase;

public class CreateUserAccountUseCaseHandler : ICreateUserAccountUseCaseHandler
{
    private readonly IUsersRepository _usersRepository;
    private readonly ISecurityCredentialsManager _securityCredentialsManager;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;

    public CreateUserAccountUseCaseHandler(
        IUsersRepository usersRepository,
        ISecurityCredentialsManager securityCredentialsManager,
        IJwtTokenGenerator jwtTokenGenerator)
    {
        _usersRepository = usersRepository;
        _securityCredentialsManager = securityCredentialsManager;
        _jwtTokenGenerator = jwtTokenGenerator;
    }

    public async Task<CreateUserAccountResult> ExecuteAsync(
        CreateUserAccountCommand command, CancellationToken cancellationToken)
    {
        if (await _usersRepository.EmailExistsAsync(command.Email, cancellationToken))
            throw new UserExistsException();

        var user = UserModel.Create(
            command.Email,
            _securityCredentialsManager.HashPassword(command.Password));

        var refreshToken = _jwtTokenGenerator.GenerateRefreshToken(user);
        var refreshTokenHash = _securityCredentialsManager.HashRefreshToken(refreshToken);

        var userSaved = await _usersRepository.CreateUserWithRefreshTokenAsync(
            user, 
            refreshTokenHash, 
            cancellationToken);

        return new CreateUserAccountResult
        {
            AccessToken = _jwtTokenGenerator.GenerateJwtToken(userSaved),
            RefreshToken = refreshToken,
            SafeUserData = userSaved.CreateSafeUserData()
        };
    }

}