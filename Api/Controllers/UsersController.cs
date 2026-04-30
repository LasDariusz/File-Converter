using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using Api.Application.Features.Files.FetchUserFilesMetadata;


namespace Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UsersController : ControllerBase
{
    private readonly IFetchUserFilesMetadataService _fetchUserFilesMetadataService;

    public UsersController(IFetchUserFilesMetadataService fetchUserFilesMetadataService)
    {
        fetchUserFilesMetadataService = _fetchUserFilesMetadataService;
    }


    [Authorize]
    [HttpGet("{userId:guid}/files")]
    public async Task<IActionResult> GetUserFilesMetadataAsync([FromRoute] Guid userId)
    {
        /*var callerGuid = GetCurrentUserGuid();

        if (callerGuid != userId) {
            return Forbid();
        }

        var serviceResult = await _fetchUserFilesMetadataService
            .GetUserFilesAsync(new FetchUserFilesModel
        {
            UserId = id
        });*/

        /*if (!serviceResult.IsSuccess)
        {
            return MapError(serviceResult.ErrorType, serviceResult.Error);
        }*/

        //return Ok(serviceResult.Value!.Files);
        return Ok();
    }

    private Guid? GetCurrentUserGuid()
    {
        var value = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        return Guid.TryParse(value, out var guid) ? guid : null;
    }

}