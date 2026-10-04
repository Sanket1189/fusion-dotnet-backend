using WebApiBackend.Models;

namespace WebApiBackend.Services.Interfaces;

public interface ITokenService
{
    TokenResult CreateToken(User user);
}
