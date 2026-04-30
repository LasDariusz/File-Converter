namespace Api.Application.Features.Auth.Register;

public interface IRegisterService
{
    Task<RegisterResponse> RegisterAsync(RegisterRequest req);
}