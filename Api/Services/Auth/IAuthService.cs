using Api.DomainModels.GenericResult;
using Api.DomainModels.Features.Register;
using Api.DomainModels.Features.RefreshToken;
using Api.DomainModels.Features.Login;

namespace Api.Services.Auth;

public interface IAuthService
{
    Task<Result<LoginModelResponse>> LoginAsync(LoginModel model);

    Task<Result<RegisterModelResponse>> RegisterAsync(RegisterModel model);

    Task<Result<RefreshTokenModelResponse>> RefreshTokenAsync(RefreshTokenModel model);
}
