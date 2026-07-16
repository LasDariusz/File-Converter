namespace Api.Modules.Files.Application.Ports;

public interface IFileValidator
{
    bool Validate(string extension, long sizeBytes);
}