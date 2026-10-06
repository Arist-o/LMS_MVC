using System;
using Identity.Domain.Common;

namespace Identity.Domain.Entities;

public class User : BaseEntity
{
    public string Email { get; private set; }
    public string PasswordHash { get; private set; }
    public string Role { get; private set; }
    public string FirstName { get; private set; }
    public string LastName { get; private set; }
    
    // For EF Core
    private User() { }

    public User(string email, string passwordHash, string role, string firstName, string lastName)
    {
        Email = email;
        PasswordHash = passwordHash;
        Role = role;
        FirstName = firstName;
        LastName = lastName;
    }
}
