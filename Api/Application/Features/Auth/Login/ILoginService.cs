namespace Api.Application.Features.Auth.Login;

public interface ILoginService
{
    Task<LoginResponse> LoginAsync(LoginRequest req);
}
