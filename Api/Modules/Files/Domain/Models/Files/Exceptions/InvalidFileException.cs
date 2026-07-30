using Api.Shared.Application.Exceptions;

namespace Api.Modules.Files.Domain.Models.Files.Exceptions;

public sealed class InvalidFileException : AppException
{
    public InvalidFileException(string message = "The supplied file or format is invalid.")
        : base(message, ErrorType.Validation)
    {
    }
}
