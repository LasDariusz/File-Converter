using Api.Modules.Users.Application.Ports;
using Api.Modules.Users.Infrastructure.Adapters;
using Api.Modules.Users.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Api.Modules.Users.Infrastructure.Config;

public static class UsersInfrastructureExtensions
{
    public static void ConfigureUsersInfrastructure(this IHostApplicationBuilder builder)
    {
        builder.ConfigureUsersAuthentication();

        builder.Services.AddScoped<IUsersRepository, UsersRepository>();
        builder.Services.AddTransient<IJwtTokenGenerator, JwtTokenGenerator>();
        builder.Services.AddTransient<ISecurityCredentialsManager, SecurityCredentialsManager>();
    }

    private static void ConfigureUsersAuthentication(this IHostApplicationBuilder builder)
    {
        var jwtSection = builder.Configuration.GetSection("JwtConfig");
        var jwtOptions = jwtSection.Get<JwtConfigOptions>()
            ?? throw new InvalidOperationException("JwtConfig section is missing");

        builder.Services.Configure<JwtConfigOptions>(jwtSection);

        builder.Services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = jwtOptions.ValidateIssuer,
                    ValidateAudience = jwtOptions.ValidateAudience,
                    ValidateLifetime = jwtOptions.ValidateLifetime,
                    ValidateIssuerSigningKey = jwtOptions.ValidateIssuerSigningKey,
                    ValidIssuer = jwtOptions.Issuer,
                    ValidAudience = jwtOptions.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.Secret))
                };
            });
    }
}