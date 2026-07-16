namespace Api.Modules.Users.Application.UseCases.LoginUserUseCase;

public interface ILoginUserUseCaseHandler
{
    Task<LoginUserResult> ExecuteAsync(
        LoginUserCommand command,
        CancellationToken cancellationToken);
}