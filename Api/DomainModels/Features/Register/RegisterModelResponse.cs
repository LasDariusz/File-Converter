namespace Api.DomainModels.Features.Register;

public class RegisterModelResponse
{
    public required Guid PublicId { get; set; }
    public required string Username { get; set; }
    public required string Email { get; set; }
    public required string Token { get; set; }
}