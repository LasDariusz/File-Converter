using Api.DomainModels.Features.FetchFile;
using Api.DomainModels.Features.UserFilesModel;
using Api.DomainModels.GenericResult;

namespace Api.Services.Users;

public interface IUsersService
{
    Task<Result<FetchUserFilesModelResponse>> GetUserFilesAsync(FetchUserFilesModel model);
}
