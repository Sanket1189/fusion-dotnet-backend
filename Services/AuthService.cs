using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WebApiBackend.Data;
using WebApiBackend.DTOs.Auth;
using WebApiBackend.DTOs.Users;
using WebApiBackend.Models;
using WebApiBackend.Services.Interfaces;

namespace WebApiBackend.Services;

public class AuthService : IAuthService
{
    private readonly FusionContext _context;
    private readonly IPasswordHasher<User> _passwordHasher;
    private readonly ITokenService _tokenService;

    public AuthService(
        FusionContext context,
        IPasswordHasher<User> passwordHasher,
        ITokenService tokenService)
    {
        _context = context;
        _passwordHasher = passwordHasher;
        _tokenService = tokenService;
    }

    public async Task<AuthServiceResult> RegisterAsync(RegisterRequestDto request)
    {
        var normalizedEmail = request.Email.Trim().ToLowerInvariant();
        var emailExists = await _context.Users
            .AnyAsync(user => user.Email == normalizedEmail);

        if (emailExists)
        {
            return new AuthServiceResult(AuthServiceStatus.DuplicateEmail, null);
        }

        var user = new User
        {
            FirstName = request.FirstName.Trim(),
            LastName = request.LastName.Trim(),
            Email = normalizedEmail,
            Role = "User",
            DateOfBirth = request.DateOfBirth
        };

        user.PasswordHash = _passwordHasher.HashPassword(user, request.Password);

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return new AuthServiceResult(AuthServiceStatus.Success, CreateResponse(user));
    }

    public async Task<AuthServiceResult> LoginAsync(LoginRequestDto request)
    {
        var normalizedEmail = request.Email.Trim().ToLowerInvariant();
        var user = await _context.Users
            .FirstOrDefaultAsync(existingUser => existingUser.Email == normalizedEmail);

        if (user is null || string.IsNullOrWhiteSpace(user.PasswordHash))
        {
            return new AuthServiceResult(AuthServiceStatus.InvalidCredentials, null);
        }

        var verificationResult = _passwordHasher.VerifyHashedPassword(
            user,
            user.PasswordHash,
            request.Password);

        if (verificationResult == PasswordVerificationResult.Failed)
        {
            return new AuthServiceResult(AuthServiceStatus.InvalidCredentials, null);
        }

        if (verificationResult == PasswordVerificationResult.SuccessRehashNeeded)
        {
            user.PasswordHash = _passwordHasher.HashPassword(user, request.Password);
            await _context.SaveChangesAsync();
        }

        return new AuthServiceResult(AuthServiceStatus.Success, CreateResponse(user));
    }

    private AuthResponseDto CreateResponse(User user)
    {
        var token = _tokenService.CreateToken(user);

        return new AuthResponseDto
        {
            Token = token.Token,
            ExpiresAt = token.ExpiresAt,
            User = new UserResponseDto
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Role = user.Role,
                DateOfBirth = user.DateOfBirth,
                CreatedAt = user.CreatedAt,
                UpdatedAt = user.UpdatedAt
            }
        };
    }
}
