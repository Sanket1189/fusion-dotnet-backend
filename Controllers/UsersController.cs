using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApiBackend.DTOs.Users;
using WebApiBackend.Services;
using WebApiBackend.Services.Interfaces;

namespace WebApiBackend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet]
    public async Task<ActionResult<PagedResultDto<UserResponseDto>>> GetUsers(
        [FromQuery] UserQueryDto queryDto)
    {
        var result = await _userService.GetUsersAsync(queryDto);
        return Ok(result);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<UserResponseDto>> GetUserById(int id)
    {
        var user = await _userService.GetUserByIdAsync(id);

        if (user is null)
        {
            return NotFound(new { message = $"User with ID {id} was not found." });
        }

        return Ok(user);
    }

    [HttpPost]
    public async Task<ActionResult<UserResponseDto>> CreateUser([FromBody] CreateUserDto userDto)
    {
        var result = await _userService.CreateUserAsync(userDto);

        if (result.Status == UserServiceStatus.DuplicateEmail)
        {
            return Conflict(new { message = $"A user with email {userDto.Email} already exists." });
        }

        var createdUser = result.User!;

        return CreatedAtAction(
            nameof(GetUserById),
            new { id = createdUser.Id },
            createdUser);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<UserResponseDto>> UpdateUser(
        int id,
        [FromBody] UpdateUserDto userDto)
    {
        var result = await _userService.UpdateUserAsync(id, userDto);

        if (result.Status == UserServiceStatus.NotFound)
        {
            return NotFound(new { message = $"User with ID {id} was not found." });
        }

        if (result.Status == UserServiceStatus.DuplicateEmail)
        {
            return Conflict(new
            {
                message = $"Another user already uses email {userDto.Email}."
            });
        }

        return Ok(result.User);
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteUser(int id)
    {
        var result = await _userService.DeleteUserAsync(id);

        if (result.Status == UserServiceStatus.NotFound)
        {
            return NotFound(new { message = $"User with ID {id} was not found." });
        }

        return NoContent();
    }
}
