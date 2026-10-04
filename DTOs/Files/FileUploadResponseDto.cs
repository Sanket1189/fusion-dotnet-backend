namespace WebApiBackend.DTOs.Files;

public class FileUploadResponseDto
{
    public required string FileName { get; set; }
    public required string Url { get; set; }
    public required string ContentType { get; set; }
    public long Size { get; set; }
}
