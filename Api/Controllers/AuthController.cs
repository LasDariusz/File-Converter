using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using Api.Services.Auth;
using Api.DomainModels.GenericResult;
using Api.DomainModels.Features.Login;
using Api.DomainModels.Features.Register;
using Api.DomainModels.Features.RefreshToken;
using Api.DTOs.Features.Login;
using Api.DTOs.Features.Register;
using Api.DTOs.Features.RefreshToken;


namespace Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    private IActionResult MapError(ErrorType? errorType, string? error)
    {
        return errorType switch
        {
            ErrorType.NotFound => NotFound(error),

            ErrorType.Validation => BadRequest(error),

            ErrorType.Conflict => Conflict(error),

            ErrorType.Unauthorized => Unauthorized(error),

            ErrorType.Unknown => StatusCode(500, error),

            _ => StatusCode(500, error)
        };
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> LoginAsync([FromBody] LoginRequestDto req)
    {
        var serviceResult = await _authService.LoginAsync(new LoginModel
        {
            Email = req.Email,
            Password = req.Password
        });

        if (!serviceResult.IsSuccess)
        {
            return MapError(serviceResult.ErrorType, serviceResult.Error);
        }

        var res = new LoginResponseDto
        {
            id = serviceResult.Value.PublicId.ToString(),
            username = serviceResult.Value.Username,
            email = serviceResult.Value.Email,
            token = serviceResult.Value.Token
        };

        return Ok(res);
    }

    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<IActionResult> RegisterAsync([FromBody] RegisterRequestDto req)
    {
        var serviceResult = await _authService.RegisterAsync(new RegisterModel
        {
            Email = req.Email,
            Username = req.Username,
            Password = req.Password
        });

        if (!serviceResult.IsSuccess)
        {
            return MapError(serviceResult.ErrorType, serviceResult.Error);
        }

        var res = new RegisterResponseDto
        {
            id = serviceResult.Value.PublicId.ToString(),
            username = serviceResult.Value.Username,
            email = serviceResult.Value.Email,
            token = serviceResult.Value.Token
        };

        return Ok(res);
    }

    [Authorize]
    [HttpPost("refresh")]
    public async Task<IActionResult> RefreshTokenAsync([FromBody] LoginRequestDto req)
    {
        throw new NotImplementedException();
    }

}
