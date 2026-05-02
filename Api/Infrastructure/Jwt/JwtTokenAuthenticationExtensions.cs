using Api.Options;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Api.Infrastructure.Jwt;

public static class JwtTokenAuthenticationExtensions
{
    public static void ConfigureTokenAuthentication(this IHostApplicationBuilder builder)
    {
        var jwtOptions = builder.Configuration
            .GetSection("JwtConfig")
            .Get<JwtConfigOptions>()!;

        builder.Services.Configure<JwtConfigOptions>(
            builder.Configuration.GetSection("JwtConfig")
        );

        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
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

        builder.Services.AddTransient<IJwtTokenGenerator, JwtTokenGenerator>();
    }
}