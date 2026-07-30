namespace Api.Modules.Files.Domain.Models.Conversions;

public enum ConversionStatus
{
    Queued,
    Processing,
    Completed,
    Failed,
    Cancelled
}