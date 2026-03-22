using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using Api.Services.Users;
using Api.DomainModels.GenericResult;
using Api.DomainModels.Features.UserFilesModel;


namespace Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UsersController : ControllerBase
{
    private readonly IUsersService _usersService;

    public UsersController(IUsersService usersService)
    {
        _usersService = usersService;
    }


    private Guid? GetCurrentUserGuid()
    {
        var value = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        return Guid.TryParse(value, out var guid) ? guid : null;
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


    [Authorize]
    [HttpGet("{id:guid}/files")]
    public async Task<IActionResult> GetUserFilesAsync([FromRoute] Guid id)
    {
        // Użytkownik może pobierać tylko własne pliki
        var callerGuid = GetCurrentUserGuid();
        if (callerGuid == null) return Unauthorized();
        if (callerGuid.Value != id) return Forbid();

        var serviceResult = await _usersService.GetUserFilesAsync(new FetchUserFilesModel
        {
            UserId = id
        });

        if (!serviceResult.IsSuccess)
        {
            return MapError(serviceResult.ErrorType, serviceResult.Error);
        }

        return Ok(serviceResult.Value!.Files);
    }

}