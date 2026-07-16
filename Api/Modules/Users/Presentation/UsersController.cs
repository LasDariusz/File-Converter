using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;



namespace Api.Modules.Users.Presentation;

[Route("api/[controller]")]
[ApiController]
public class UsersController : ControllerBase
{
    /*private readonly IFetchFilesMetadataService _fetchUserFilesMetadataService;

    public UsersController(IFetchFilesMetadataService fetchUserFilesMetadataService)
    {
        fetchUserFilesMetadataService = _fetchUserFilesMetadataService;
    }

    private Guid? GetCurrentUserGuid()
    {
        var value = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        return Guid.TryParse(value, out var guid) ? guid : null;
    }*/

}