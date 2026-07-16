namespace Api.Modules.Users.Application.UseCases.CreateUserAccountUseCase;

public interface ICreateUserAccountUseCaseHandler
{
    Task<CreateUserAccountResult> ExecuteAsync(
        CreateUserAccountCommand command,
        CancellationToken cancellationToken);
}