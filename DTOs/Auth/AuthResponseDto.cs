using WebApiBackend.DTOs.Users;

namespace WebApiBackend.DTOs.Auth;

public class AuthResponseDto
{
    public required string Token { get; set; }
    public DateTime ExpiresAt { get; set; }
    public required UserResponseDto User { get; set; }
}
