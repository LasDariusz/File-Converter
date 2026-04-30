namespace Api.Application.Validation;

public interface IFileValidator
{
    bool Validate(string extension, long sizeBytes);
}