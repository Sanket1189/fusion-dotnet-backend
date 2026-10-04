using System.ComponentModel.DataAnnotations;
using WebApiBackend.Validation;

namespace WebApiBackend.DTOs.Users;

public class UpdateUserDto
{
    [Required]
    [StringLength(50, MinimumLength = 2)]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    [StringLength(50, MinimumLength = 2)]
    public string LastName { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [StringLength(100)]
    public string Email { get; set; } = string.Empty;

    [NotInFuture]
    public DateOnly? DateOfBirth { get; set; }
}
