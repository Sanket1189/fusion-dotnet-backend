using WebApiBackend.DTOs.Auth;

namespace WebApiBackend.Services.Interfaces;

public interface IAuthService
{
    Task<AuthServiceResult> RegisterAsync(RegisterRequestDto request);
    Task<AuthServiceResult> LoginAsync(LoginRequestDto request);
}
