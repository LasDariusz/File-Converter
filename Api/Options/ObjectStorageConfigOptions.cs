namespace Api.Options;

public class ObjectStorageConfigOptions
{ 
    public required string Endpoint { get; set; }

    public required string AccessKey { get; set; }

    public required string SecretKey { get; set; }

    public required bool WithSSL { get; set; }
}