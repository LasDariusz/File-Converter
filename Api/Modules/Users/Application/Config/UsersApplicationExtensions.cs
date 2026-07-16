using Api.Modules.Users.Application.UseCases.CreateUserAccountUseCase;
using Api.Modules.Users.Application.UseCases.LoginUserUseCase;

namespace Api.Modules.Users.Application.Config;

public static class UsersApplicationExtensions
{
    public static void ConfigureUsersApplication(this IHostApplicationBuilder builder)
    {
        builder.Services.AddScoped<ICreateUserAccountUseCaseHandler, CreateUserAccountUseCaseHandler>();
        
        builder.Services.AddScoped<ILoginUserUseCaseHandler, LoginUserUseCaseHandler>();
    }
}