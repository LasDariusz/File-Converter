namespace Api.Modules.Users.Application.UseCases.LoginUserUseCase;

public record class LoginUserCommand
{
    public required string Email { get; set; }

    public required string Password { get; set; }
}