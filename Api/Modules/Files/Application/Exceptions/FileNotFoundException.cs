using Api.Shared.Application.Exceptions;

namespace Api.Modules.Files.Application.Exceptions;

public sealed class FileNotFoundException : AppException
{
    public FileNotFoundException()
        : base("The requested file was not found.", ErrorType.NotFound)
    {
    }
}
