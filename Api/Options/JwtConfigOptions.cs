namespace Api.Options;

public class JwtConfigOptions
{
    public required string Secret { get; set; }

    public required string Issuer { get; set; }

    public required string Audience { get; set; }

    public required bool ValidateIssuer { get; set; }

    public required bool ValidateAudience { get; set; }

    public required bool ValidateLifetime { get; set; }

    public required bool ValidateIssuerSigningKey { get; set; }

    public required int AccessTokenValidMinutes { get; set; }

    public required int RefreshTokenValidDays { get; set; }
}