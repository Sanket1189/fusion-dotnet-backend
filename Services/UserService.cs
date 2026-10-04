using Microsoft.EntityFrameworkCore;
using WebApiBackend.Data;
using WebApiBackend.DTOs.Users;
using WebApiBackend.Models;
using WebApiBackend.Services.Interfaces;

namespace WebApiBackend.Services;

public class UserService : IUserService
{
    private readonly FusionContext _context;

    public UserService(FusionContext context)
    {
        _context = context;
    }

    public async Task<PagedResultDto<UserResponseDto>> GetUsersAsync(UserQueryDto queryDto)
    {
        var query = _context.Users.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(queryDto.Search))
        {
            var search = queryDto.Search.Trim();

            query = query.Where(user =>
                user.FirstName.Contains(search) ||
                user.LastName.Contains(search) ||
                user.Email.Contains(search));
        }

        query = queryDto.SortBy.Trim().ToLowerInvariant() switch
        {
            "firstname" => queryDto.Descending
                ? query.OrderByDescending(user => user.FirstName)
                : query.OrderBy(user => user.FirstName),
            "lastname" => queryDto.Descending
                ? query.OrderByDescending(user => user.LastName)
                : query.OrderBy(user => user.LastName),
            "email" => queryDto.Descending
                ? query.OrderByDescending(user => user.Email)
                : query.OrderBy(user => user.Email),
            "createdat" => queryDto.Descending
                ? query.OrderByDescending(user => user.CreatedAt)
                : query.OrderBy(user => user.CreatedAt),
            _ => queryDto.Descending
                ? query.OrderByDescending(user => user.Id)
                : query.OrderBy(user => user.Id)
        };

        var totalItems = await query.CountAsync();

        var users = await query
            .Skip((queryDto.Page - 1) * queryDto.PageSize)
            .Take(queryDto.PageSize)
            .Select(user => new UserResponseDto
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Role = user.Role,
                DateOfBirth = user.DateOfBirth,
                CreatedAt = user.CreatedAt,
                UpdatedAt = user.UpdatedAt
            })
            .ToListAsync();

        return new PagedResultDto<UserResponseDto>
        {
            Items = users,
            Page = queryDto.Page,
            PageSize = queryDto.PageSize,
            TotalItems = totalItems,
            TotalPages = (int)Math.Ceiling(totalItems / (double)queryDto.PageSize)
        };
    }

    public async Task<UserResponseDto?> GetUserByIdAsync(int id)
    {
        return await _context.Users
            .AsNoTracking()
            .Where(user => user.Id == id)
            .Select(user => new UserResponseDto
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Role = user.Role,
                DateOfBirth = user.DateOfBirth,
                CreatedAt = user.CreatedAt,
                UpdatedAt = user.UpdatedAt
            })
            .FirstOrDefaultAsync();
    }

    public async Task<UserServiceResult> CreateUserAsync(CreateUserDto userDto)
    {
        var emailExists = await _context.Users
            .AnyAsync(existingUser => existingUser.Email == userDto.Email);

        if (emailExists)
        {
            return new UserServiceResult(UserServiceStatus.DuplicateEmail, null);
        }

        var user = new User
        {
            FirstName = userDto.FirstName,
            LastName = userDto.LastName,
            Email = userDto.Email,
            DateOfBirth = userDto.DateOfBirth
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        return new UserServiceResult(UserServiceStatus.Success, MapToResponseDto(user));
    }

    public async Task<UserServiceResult> UpdateUserAsync(int id, UpdateUserDto userDto)
    {
        var existingUser = await _context.Users
            .FirstOrDefaultAsync(user => user.Id == id);

        if (existingUser is null)
        {
            return new UserServiceResult(UserServiceStatus.NotFound, null);
        }

        var emailBelongsToAnotherUser = await _context.Users
            .AnyAsync(user => user.Email == userDto.Email && user.Id != id);

        if (emailBelongsToAnotherUser)
        {
            return new UserServiceResult(UserServiceStatus.DuplicateEmail, null);
        }

        existingUser.FirstName = userDto.FirstName;
        existingUser.LastName = userDto.LastName;
        existingUser.Email = userDto.Email;
        existingUser.DateOfBirth = userDto.DateOfBirth;

        await _context.SaveChangesAsync();
        await _context.Entry(existingUser).ReloadAsync();

        return new UserServiceResult(UserServiceStatus.Success, MapToResponseDto(existingUser));
    }

    public async Task<UserServiceResult> DeleteUserAsync(int id)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(existingUser => existingUser.Id == id);

        if (user is null)
        {
            return new UserServiceResult(UserServiceStatus.NotFound, null);
        }

        _context.Users.Remove(user);
        await _context.SaveChangesAsync();

        return new UserServiceResult(UserServiceStatus.Success, MapToResponseDto(user));
    }

    private static UserResponseDto MapToResponseDto(User user)
    {
        return new UserResponseDto
        {
            Id = user.Id,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email,
            Role = user.Role,
            DateOfBirth = user.DateOfBirth,
            CreatedAt = user.CreatedAt,
            UpdatedAt = user.UpdatedAt
        };
    }
}
