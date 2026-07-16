using Api.Modules.Files.Domain.Models.Files;
using Api.Modules.Files.Domain.Models.Files.Exceptions;

namespace Api.Modules.Files.Application.Common;

public class IncomingFile
{
    private readonly Func<Stream> _openReadStream;

    public FileMetadata Metadata { get; }

    public string FileName => Metadata.FileName;

    public FileFormat FileFormat => Metadata.FileFormat;

    public long SizeBytes => Metadata.SizeBytes;

    public string? DeclaredContentType { get; }

    private IncomingFile(
       FileMetadata metadata,
       string? declaredContentType,
       Func<Stream> openReadStream)
    {
        Metadata = metadata;
        DeclaredContentType = declaredContentType;
        _openReadStream = openReadStream;
    }

    public static IncomingFile Create(
        string fileName,
        string? declaredContentType,
        long sizeBytes,
        Func<Stream> openReadStream)
    {
        if (openReadStream == null)
            throw new ArgumentNullException();

        var metadata = FileMetadata.Create(fileName, sizeBytes);

        return new IncomingFile(
            metadata, 
            declaredContentType, 
            openReadStream);
    }

    public Stream OpenReadStream()
    {
        var stream = _openReadStream() ?? 
            throw new InvalidOperationException();

        if (!stream.CanRead)
        {
            stream.Dispose();
            throw new InvalidFileException();
        }

        return stream;
    }

}