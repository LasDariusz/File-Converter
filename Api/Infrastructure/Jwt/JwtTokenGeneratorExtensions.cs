using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;


namespace Api.Infrastructure.Jwt;

public static class JwtTokenGeneratorExtensions
{
    public static void ConfigureTokenGeneration(this IHostApplicationBuilder builder)
    {
        var jwt = "SuperExtraSecretJwtKey1234567890";

        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt))
                };
            });

        builder.Services.AddTransient<IJwtTokenGenerator, JwtTokenGenerator>();
    }
}