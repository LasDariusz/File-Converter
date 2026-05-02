using Api.Infrastructure.Database.Context;
using Api.Options;
using Microsoft.EntityFrameworkCore;

namespace Api.Infrastructure.Database;

public static class DatabaseContextExtensions
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