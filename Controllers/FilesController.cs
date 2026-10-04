using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApiBackend.DTOs.Files;
using WebApiBackend.Services;
using WebApiBackend.Services.Interfaces;

namespace WebApiBackend.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class FilesController : ControllerBase
{
    private readonly IFileStorageService _fileStorageService;

    public FilesController(IFileStorageService fileStorageService)
    {
        _fileStorageService = fileStorageService;
    }

    [HttpPost("images")]
    [Consumes("multipart/form-data")]
    [RequestSizeLimit(6 * 1024 * 1024)]
    public async Task<ActionResult<FileUploadResponseDto>> UploadImage(
        IFormFile file,
        CancellationToken cancellationToken)
    {
        var result = await _fileStorageService.SaveImageAsync(
            file,
            cancellationToken);

        if (result.Status == ImageUploadStatus.EmptyFile)
        {
            return BadRequest(new { message = "Please choose a non-empty image." });
        }

        if (result.Status == ImageUploadStatus.TooLarge)
        {
            return StatusCode(StatusCodes.Status413PayloadTooLarge, new
            {
                message = "The image must be 5 MB or smaller."
            });
        }

        if (result.Status == ImageUploadStatus.UnsupportedType)
        {
            return StatusCode(StatusCodes.Status415UnsupportedMediaType, new
            {
                message = "Only JPG, PNG, and WebP images are allowed."
            });
        }

        if (result.Status == ImageUploadStatus.InvalidContent)
        {
            return BadRequest(new
            {
                message = "The file contents do not match a supported image format."
            });
        }

        var response = new FileUploadResponseDto
        {
            FileName = result.FileName!,
            Url = $"{Request.Scheme}://{Request.Host}{result.RelativeUrl}",
            ContentType = result.ContentType!,
            Size = result.Size
        };

        return Created(response.Url, response);
    }
}
