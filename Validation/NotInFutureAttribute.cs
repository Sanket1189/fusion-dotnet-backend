using System.ComponentModel.DataAnnotations;

namespace WebApiBackend.Validation;

public class NotInFutureAttribute : ValidationAttribute
{
    public NotInFutureAttribute()
    {
        ErrorMessage = "Date of birth cannot be in the future.";
    }

    public override bool IsValid(object? value)
    {
        return value is null || value is DateOnly date && date <= DateOnly.FromDateTime(DateTime.Today);
    }
}
