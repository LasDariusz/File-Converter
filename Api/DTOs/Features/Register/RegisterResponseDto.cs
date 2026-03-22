namespace Api.DTOs.Features.Register;

public class RegisterResponseDto
{
    public required string id { get; set; }
    public required string username { get; set; }
    public required string email { get; set; }
    public required string token { get; set; }
}
