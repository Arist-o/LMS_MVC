namespace Identity.Application.Users.DTOs;

public record UserDto(
    Guid Id,
    string Email,
    string FirstName,
    string LastName,
    string Role,
    bool IsBlocked
);
