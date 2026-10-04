using WebApiBackend.DTOs.Users;

namespace WebApiBackend.Services;

public enum UserServiceStatus
{
    Success,
    NotFound,
    DuplicateEmail
}

public record UserServiceResult(UserServiceStatus Status, UserResponseDto? User);
