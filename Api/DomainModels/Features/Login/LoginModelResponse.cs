namespace Api.DomainModels.Features.Login;

public class LoginModelResponse
{
    public required Guid PublicId { get; set; }
    public required string Username { get; set; }
    public required string Email { get; set; }
    public required string Token { get; set; }
}
