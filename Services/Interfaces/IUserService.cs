using WebApiBackend.DTOs.Users;
using WebApiBackend.Services;

namespace WebApiBackend.Services.Interfaces;

public interface IUserService
{
    Task<PagedResultDto<UserResponseDto>> GetUsersAsync(UserQueryDto queryDto);
    Task<UserResponseDto?> GetUserByIdAsync(int id);
    Task<UserServiceResult> CreateUserAsync(CreateUserDto userDto);
    Task<UserServiceResult> UpdateUserAsync(int id, UpdateUserDto userDto);
    Task<UserServiceResult> DeleteUserAsync(int id);
}
