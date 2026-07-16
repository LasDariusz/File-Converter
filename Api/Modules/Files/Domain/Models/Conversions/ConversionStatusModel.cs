namespace Api.Modules.Files.Domain.Models.Conversions;

public enum ConversionStatusModel
{
    Queued,
    Processing,
    Completed,
    Failed,
    Cancelled
}