namespace Api.Modules.Users.Application.UseCases.CreateUserAccountUseCase;

public record class CreateUserAccountCommand
{
    public required string Email { get; set; }

    public required string Password { get; set; }
}