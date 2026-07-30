using Api.Shared.Application.Exceptions;

namespace Api.Modules.Files.Application.Exceptions;

public sealed class FileOwnershipException : AppException
{
    public FileOwnershipException()
        : base("The file does not belong to the current user.", ErrorType.Forbidden)
    {
    }
}
