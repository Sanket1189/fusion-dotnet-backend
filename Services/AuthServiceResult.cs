using WebApiBackend.DTOs.Auth;

namespace WebApiBackend.Services;

public enum AuthServiceStatus
{
    Success,
    DuplicateEmail,
    InvalidCredentials
}

public record AuthServiceResult(AuthServiceStatus Status, AuthResponseDto? Response);
