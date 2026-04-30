using Api.Infrastructure.Database.Context;
using Microsoft.EntityFrameworkCore;

namespace Api.Infrastructure.Database;

public static class DatabaseContextExtensions
{
    public static void ConfigureDatabaseContext(this IHostApplicationBuilder builder)
    {
        builder.Services.AddDbContext<FileConverterContext>(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
        );
    }
}