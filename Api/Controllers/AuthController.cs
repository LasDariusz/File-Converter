using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using Api.Application.Features.Auth.Login;
using Api.Application.Features.Auth.Register;

using Api.DTOs.Requests;
using Api.DTOs.Responses;
using Api.DTOs.Shared;


namespace Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly ILoginService _loginService;
    private readonly IRegisterService _registerService;

    public AuthController(ILoginService loginService, IRegisterService registerService)
    {
        _loginService = loginService;
        _registerService = registerService;
    }


    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> LoginAsync([FromBody] LoginRequestDto req)
    {
        var serviceResult = await _loginService.LoginAsync(new LoginRequest
        {
            Email = req.Email,
            Password = req.Password
        });

        var res = new LoginResponseDto
        {
            AccessToken = serviceResult.AccessToken,
            ExpiresInMinutes = serviceResult.ExpiresInMinutes,
            RefreshToken = serviceResult.RefreshToken,
            UserData = new AuthenticatedUserDataDto
            { 
                UserId = serviceResult.UserData.UserId,
                Email = serviceResult.UserData.Email,
                Username = serviceResult.UserData.Username
            }
        };

        return Ok(res);
    }

    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<IActionResult> RegisterAsync([FromBody] RegisterRequestDto req)
    {
        var serviceResult = await _registerService.RegisterAsync(new RegisterRequest
        {
            Email = req.Email,
            Username= req.Username,
            Password = req.Password
        });

        var res = new RegisterResponseDto
        {
            AccessToken = serviceResult.AccessToken,
            ExpiresInMinutes = serviceResult.ExpiresInMinutes,
            RefreshToken = serviceResult.RefreshToken,
            UserData = new AuthenticatedUserDataDto
            {
                UserId = serviceResult.UserData.UserId,
                Email = serviceResult.UserData.Email,
                Username = serviceResult.UserData.Email
            }
        };

        return Created($"api/Users/{res.UserData.UserId}", res);
    }

    [Authorize]
    [HttpPost("refresh")]
    public async Task<IActionResult> RefreshTokenAsync([FromBody] RefreshTokenRequestDto req)
    {
        throw new NotImplementedException();
    }

}