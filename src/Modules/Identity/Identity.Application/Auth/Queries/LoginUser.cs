using FluentValidation;
using Identity.Application.Auth.DTOs;
using Identity.Application.Auth.Services;
using Identity.Application.Common.Interfaces;
using Identity.Application.Common.Models;
using Identity.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Identity.Application.Auth.Queries;

public static class LoginUser
{
    public sealed record Query(LoginDto Dto) : IRequest<Result<TokenResponseDto>>;

    public sealed class Validator : AbstractValidator<Query>
    {
        public Validator()
        {
            RuleFor(x => x.Dto.Email).NotEmpty().EmailAddress();
            RuleFor(x => x.Dto.Password).NotEmpty();
        }
    }

    public sealed class Handler(IIdentityUnitOfWork unitOfWork, IJwtProvider jwtProvider) : IRequestHandler<Query, Result<TokenResponseDto>>
    {
        public async Task<Result<TokenResponseDto>> Handle(Query request, CancellationToken cancellationToken)
        {
            var user = await unitOfWork.Users.FirstOrDefaultAsync(u => u.Email == request.Dto.Email, cancellationToken);
            if (user == null)
            {
                return Result<TokenResponseDto>.Failure("Invalid credentials.");
            }

            var passwordHasher = new PasswordHasher<User>();
            var result = passwordHasher.VerifyHashedPassword(user, user.PasswordHash, request.Dto.Password);
            if (result == PasswordVerificationResult.Failed)
            {
                return Result<TokenResponseDto>.Failure("Invalid credentials.");
            }

            var token = jwtProvider.Generate(user);
            return Result<TokenResponseDto>.Success(new TokenResponseDto(token, user.Email, user.Role, user.FirstName, user.LastName));
        }
    }
}
