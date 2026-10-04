namespace WebApiBackend.Services;

public enum ImageUploadStatus
{
    Success,
    EmptyFile,
    TooLarge,
    UnsupportedType,
    InvalidContent
}

public record ImageUploadResult(
    ImageUploadStatus Status,
    string? FileName = null,
    string? RelativeUrl = null,
    string? ContentType = null,
    long Size = 0);
