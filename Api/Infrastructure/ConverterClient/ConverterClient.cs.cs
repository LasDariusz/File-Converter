namespace Api.Infrastructure.ConverterClient;

public class ConverterClient : IConverterClient
{

    private readonly HttpClient _httpClient;

    public ConverterClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<bool> ConvertFileAsync(
        string inputFileStorageKey, 
        string outputFileStorageKey, 
        string targetFormat)
    {
        var requestPayload = new
        {
            inputFileStorageKey = inputFileStorageKey,
            outputFileStorageKey = outputFileStorageKey,
            targetFormat = targetFormat
        };
        
        try 
        {
            var response = await _httpClient.PostAsJsonAsync("/convert", requestPayload);
            return response.IsSuccessStatusCode;
        } 
        catch (Exception ex) 
        {
            return false;
        }
    }

}
