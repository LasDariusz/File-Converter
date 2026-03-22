using Api.DomainModels.GenericResult;
using Api.DomainModels.Features.ConvertFile;
using Api.DomainModels.Features.DeleteFile;
using Api.DomainModels.Features.UploadFile;
using Api.DomainModels.Features.FetchFile;

namespace Api.Services.Files;

public interface IFilesService
{
    Task<Result<FetchFileModelResponse>> GetFileByGuidAsync(FetchFileModel model);

    Task<Result<UploadFileModelResponse>> UploadFileAsync(UploadFileModel model);
    
    Task<Result<ConvertFileModelResponse>> ConvertFileToSpecifiedFormatAsync(ConvertFileModel model);

    Task<Result<DeleteFileModelResponse>> DeleteFileByGuidAsync(DeleteFileModel model);
}
