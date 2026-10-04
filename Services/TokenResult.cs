namespace WebApiBackend.Services;

public record TokenResult(string Token, DateTime ExpiresAt);
