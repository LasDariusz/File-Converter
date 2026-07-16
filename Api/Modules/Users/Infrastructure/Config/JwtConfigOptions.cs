namespace Api.Modules.Users.Infrastructure.Config;

public class JwtConfigOptions
{
    public required string Secret { get; set; }

    public required string Issuer { get; set; }

    public required string Audience { get; set; }

    public required bool ValidateIssuer { get; set; } = true;

    public required bool ValidateAudience { get; set; } = true;

    public required bool ValidateLifetime { get; set; } = true;

    public required bool ValidateIssuerSigningKey { get; set; } = true;

    public required int AccessTokenValidMinutes { get; set; }

    public required int RefreshTokenValidDays { get; set; }
}