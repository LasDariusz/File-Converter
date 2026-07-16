using Api.Modules.Users.Application.Exceptions;
using Api.Modules.Users.Application.Ports;

namespace Api.Modules.Users.Application.UseCases.LoginUserUseCase;

public class LoginUserUseCaseHandler : ILoginUserUseCaseHandler
{
    private readonly IUsersRepository _usersRepository;
    private readonly ISecurityCredentialsManager _securityCredentialsManager;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;

    public LoginUserUseCaseHandler(
        IUsersRepository usersRepository,
        ISecurityCredentialsManager securityCredentialsManager,
        IJwtTokenGenerator jwtTokenGenerator)
    {
        _usersRepository = usersRepository;
        _securityCredentialsManager = securityCredentialsManager;
        _jwtTokenGenerator = jwtTokenGenerator;
    }

    public async Task<LoginUserResult> ExecuteAsync(
        LoginUserCommand command, 
        CancellationToken cancellationToken)
    {
        var user = await _usersRepository
            .FindUserByEmailAsync(command.Email, cancellationToken);

        if (user == null)
            throw new UserNotFoundException();

        if (!_securityCredentialsManager.VerifyPassword(command.Password, user))
            throw new InvalidCredentialsException();

        var refreshToken = _jwtTokenGenerator
            .GenerateRefreshToken(user);

        var refreshTokenHash = _securityCredentialsManager
            .HashRefreshToken(refreshToken);

        var tokenSaved = await _usersRepository.SaveUserRefreshTokenAsync(
            user, 
            refreshTokenHash, 
            cancellationToken);

        if (!tokenSaved)
            throw new Exception();

        return new LoginUserResult
        {
            AccessToken = _jwtTokenGenerator.GenerateJwtToken(user),
            RefreshToken = refreshToken,
            SafeUserData = user.CreateSafeUserData()
        };
    }

}