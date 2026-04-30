namespace Api.Options;

public class FileOptions
{
    public List<string> AllowedImageFileFormats { get; set; } = new List<string>();
    /*public List<string> AllowedVideoFileFormats { get; set; } = new List<string>();
    public List<string> AllowedAudioFileFormats { get; set; } = new List<string>();
    public List<string> AllowedDocumentFileFormats { get; set; } = new List<string>();*/

    public long MaxImageFileSizeMb { get; set; }
    /*public long MaxVideoFileSizeMb { get; set; }
    public long MaxAudioFileSizeMb { get; set; }
    public long MaxDocumentFileSizeMb { get; set; }*/
}