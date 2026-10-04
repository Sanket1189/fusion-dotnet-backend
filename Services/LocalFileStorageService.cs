using WebApiBackend.Services.Interfaces;

namespace WebApiBackend.Services;

public class LocalFileStorageService : IFileStorageService
{
    private const long MaxImageSize = 5 * 1024 * 1024;

    private static readonly Dictionary<string, string> AllowedTypes =
        new(StringComparer.OrdinalIgnoreCase)
        {
            [".jpg"] = "image/jpeg",
            [".jpeg"] = "image/jpeg",
            [".png"] = "image/png",
            [".webp"] = "image/webp"
        };

    private readonly IWebHostEnvironment _environment;

    public LocalFileStorageService(IWebHostEnvironment environment)
    {
        _environment = environment;
    }

    public async Task<ImageUploadResult> SaveImageAsync(
        IFormFile file,
        CancellationToken cancellationToken)
    {
        if (file.Length == 0)
        {
            return new ImageUploadResult(ImageUploadStatus.EmptyFile);
        }

        if (file.Length > MaxImageSize)
        {
            return new ImageUploadResult(ImageUploadStatus.TooLarge);
        }

        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();

        if (!AllowedTypes.TryGetValue(extension, out var contentType))
        {
            return new ImageUploadResult(ImageUploadStatus.UnsupportedType);
        }

        if (!await HasValidImageSignatureAsync(file, extension, cancellationToken))
        {
            return new ImageUploadResult(ImageUploadStatus.InvalidContent);
        }

        var webRoot = _environment.WebRootPath
            ?? Path.Combine(_environment.ContentRootPath, "wwwroot");
        var uploadDirectory = Path.Combine(webRoot, "uploads", "images");
        Directory.CreateDirectory(uploadDirectory);

        var safeFileName = $"{Guid.NewGuid():N}{extension}";
        var fullPath = Path.Combine(uploadDirectory, safeFileName);

        await using var outputStream = new FileStream(
            fullPath,
            FileMode.CreateNew,
            FileAccess.Write,
            FileShare.None,
            81920,
            useAsync: true);
        await file.CopyToAsync(outputStream, cancellationToken);

        return new ImageUploadResult(
            ImageUploadStatus.Success,
            safeFileName,
            $"/uploads/images/{safeFileName}",
            contentType,
            file.Length);
    }

    private static async Task<bool> HasValidImageSignatureAsync(
        IFormFile file,
        string extension,
        CancellationToken cancellationToken)
    {
        var header = new byte[12];
        await using var stream = file.OpenReadStream();
        var bytesRead = await stream.ReadAsync(header, cancellationToken);

        return extension switch
        {
            ".jpg" or ".jpeg" =>
                bytesRead >= 3 &&
                header[0] == 0xFF &&
                header[1] == 0xD8 &&
                header[2] == 0xFF,
            ".png" =>
                bytesRead >= 8 &&
                header[..8].SequenceEqual(
                    new byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A }),
            ".webp" =>
                bytesRead >= 12 &&
                header[..4].SequenceEqual("RIFF"u8.ToArray()) &&
                header[8..12].SequenceEqual("WEBP"u8.ToArray()),
            _ => false
        };
    }
}
