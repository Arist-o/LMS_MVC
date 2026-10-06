namespace Identity.Application.Auth.DTOs;

public sealed record RegisterDto(string Email, string Password, string Role, string FirstName, string LastName);
public sealed record LoginDto(string Email, string Password);
public sealed record TokenResponseDto(string Token, string Email, string Role, string FirstName, string LastName);
