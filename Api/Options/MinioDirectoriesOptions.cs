namespace Api.Options;

public class MinioDirectoriesOptions
{ 
    public required string MainBucketName { get; set; }
    public required string UserImageSubDirectpry { get; set; }
    public required string UserVideosSubDirectory { get; set; }
    public required string UserAudiosDubDirectory { get; set; }
    public required string UserDocumentsSubDirecotry { get; set; }
}
