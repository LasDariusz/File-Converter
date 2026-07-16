namespace Api.Modules.Files.Application.Ports;

public interface IConverterClient
{
    Task<bool> ConvertFileAsync(string fileStorageKey, string fileOutputKey, string targetFormat);

}
