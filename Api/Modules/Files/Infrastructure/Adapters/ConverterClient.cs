using Api.Modules.Files.Application.Ports;

namespace Api.Modules.Files.Infrastructure.Adapters;

public class ConverterClient : IConverterClient
{

    private readonly HttpClient _httpClient;

    public ConverterClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<bool> ConvertFileAsync(
        string originalFileStorageKey, 
        string outputFileStorageKey, 
        string targetExtension)
    {
        var requestPayload = new
        {
            originalFileStorageKey = originalFileStorageKey,
            outputFileStorageKey = outputFileStorageKey,
            targetExtension = targetExtension
        };
        
        try 
        {
            var response = await _httpClient.PostAsJsonAsync("/convert", requestPayload);
            return response.IsSuccessStatusCode;
        } 
        catch (Exception ex) 
        {
            throw new Exception($"Failed to reach service: {ex.Message}", ex);
        }
    }

}