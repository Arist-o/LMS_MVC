using FluentValidation;
using Identity.Application.Auth.DTOs;
using Identity.Application.Common.Interfaces;
using Identity.Application.Common.Models;
using Identity.Domain.Entities;
using Identity.Application.Common.Behaviors;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Identity.Application.Auth.Commands;

public static class RegisterUser
{
    public sealed record Command(RegisterDto Dto) : ICommand<Result<Guid>>;

    public sealed class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.Dto.Email).NotEmpty().EmailAddress();
            RuleFor(x => x.Dto.Password).NotEmpty().MinimumLength(6);
            RuleFor(x => x.Dto.Role).NotEmpty().Must(r => r == "Student" || r == "Instructor" || r == "Admin");
            RuleFor(x => x.Dto.FirstName).NotEmpty();
            RuleFor(x => x.Dto.LastName).NotEmpty();
        }
    }

    public sealed class Handler(IIdentityUnitOfWork unitOfWork, AutoMapper.IMapper mapper) : IRequestHandler<Command, Result<Guid>>
    {
        public async Task<Result<Guid>> Handle(Command request, CancellationToken cancellationToken)
        {
            var existingUser = await unitOfWork.Users.FirstOrDefaultAsync(u => u.Email == request.Dto.Email, cancellationToken);
            if (existingUser != null)
            {
                return Result<Guid>.Failure("User with this email already exists.");
            }

            var passwordHasher = new PasswordHasher<User>();
            var user = new User(
                request.Dto.Email,
                string.Empty,
                request.Dto.Role,
                request.Dto.FirstName,
                request.Dto.LastName);

            var hash = passwordHasher.HashPassword(user, request.Dto.Password);
            
            // Wait, we need a way to set PasswordHash on User. It is private set.
            // So we can use reflection or add a method to User entity to set password hash,
            // or pass hash to constructor. Let's pass hash to constructor.
            var finalUser = new User(
                request.Dto.Email,
                hash,
                request.Dto.Role,
                request.Dto.FirstName,
                request.Dto.LastName);

            await unitOfWork.Users.AddAsync(finalUser, cancellationToken);

            return Result<Guid>.Success(finalUser.Id);
        }
    }
}
