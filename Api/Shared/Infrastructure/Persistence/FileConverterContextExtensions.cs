using Microsoft.EntityFrameworkCore;

namespace Api.Shared.Infrastructure.Persistence;

public static class FileConverterContextExtensions
{
    public static void ConfigureDatabaseContext(this IHostApplicationBuilder builder)
    {
        var dbOptions = builder.Configuration
            .GetSection("DatabaseConfig")
            .Get<DatabaseConfigOptions>()!;

        builder.Services.Configure<DatabaseConfigOptions>(
            builder.Configuration.GetSection("DatabaseConfig")
        );

        builder.Services.AddDbContext<FileConverterContext>(options =>
            options.UseSqlServer(dbOptions.DefaultConnection)
        );
    }
}