namespace Api.Services.ConverterClient;

public interface IConverterClient
{
    Task<bool> ConvertFileAsync(string fileStorageKey, string fileOutputKey, string targetFormat);
}
