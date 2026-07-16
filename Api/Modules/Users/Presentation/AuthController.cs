using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Api.Modules.Users.Presentation.Requests;
using Api.Modules.Users.Application.UseCases.LoginUserUseCase;
using Api.Modules.Users.Application.UseCases.CreateUserAccountUseCase;
using Api.Modules.Users.Presentation.Responses;

namespace Api.Modules.Users.Presentation;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly ILoginUserUseCaseHandler _loginUserUseCaseHandler;
    private readonly ICreateUserAccountUseCaseHandler _createUserAccountUseCaseHandler;

    public AuthController(
        ILoginUserUseCaseHandler loginUserUseCaseHandler, 
        ICreateUserAccountUseCaseHandler createUserAccountUseCaseHandler)
    {
        _loginUserUseCaseHandler = loginUserUseCaseHandler;
        _createUserAccountUseCaseHandler = createUserAccountUseCaseHandler;
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> LoginAsync(
        [FromBody] LoginRequestDto req, 
        CancellationToken cancellationToken)
    {
        var result = await _loginUserUseCaseHandler.ExecuteAsync(new LoginUserCommand
        {
            Email = req.Email,
            Password = req.Password
        }, cancellationToken);

        return Ok(new LoginResponseDto
        {
            AccessToken = result.AccessToken.Value,
            AccessTokenExpiresInMinutes = result.AccessToken.ExpiresInMinutes,
            RefreshToken = result.RefreshToken.Value,
            RefreshTokenExpiresInHours = result.RefreshToken.ExpiresInMinutes,
            UserId = result.SafeUserData.UserId,
            Email = result.SafeUserData.Email
        });
    }

    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<IActionResult> RegisterAsync(
        [FromBody] RegisterRequestDto req,
        CancellationToken cancellationToken)
    {
        var result = await _createUserAccountUseCaseHandler.ExecuteAsync(new CreateUserAccountCommand
        {
            Email = req.Email,
            Password = req.Password
        }, cancellationToken);

        return Created($"api/Users/{result.SafeUserData.UserId}", new CreateAccountResponseDto
        {
            AccessToken = result.AccessToken.Value,
            AccessTokenExpiresInMinutes = result.AccessToken.ExpiresInMinutes,
            RefreshToken = result.RefreshToken.Value,
            RefreshTokenExpiresInHours = result.RefreshToken.ExpiresInMinutes,
            UserId = result.SafeUserData.UserId,
            Email = result.SafeUserData.Email
        });
    }

    [Authorize]
    [HttpPost("refresh")]
    public async Task<IActionResult> RefreshTokenAsync(
        [FromBody] RefreshTokenRequestDto req,
        CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

}