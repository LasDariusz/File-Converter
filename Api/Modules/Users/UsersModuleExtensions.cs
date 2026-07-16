using Api.Modules.Users.Application.Config;
using Api.Modules.Users.Infrastructure.Config;

namespace Api.Modules.Users;

public static class UsersModuleExtensions
{
    public static void ConfigureUsersModule(this IHostApplicationBuilder builder)
    {
        builder.ConfigureUsersApplication();

        builder.ConfigureUsersInfrastructure();
    }
}