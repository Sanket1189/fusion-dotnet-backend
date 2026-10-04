namespace WebApiBackend.Services.Interfaces;

public interface IFileStorageService
{
    Task<ImageUploadResult> SaveImageAsync(
        IFormFile file,
        CancellationToken cancellationToken);
}
