using Identity.Application.Common.Interfaces;
using Identity.Application.Common.Models;
using MediatR;
using FluentValidation;

namespace Identity.Application.Users.Commands;

public static class ChangeUserRole
{
    public sealed record Command(Guid UserId, string Role) : IRequest<Result<bool>>;

    public sealed class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.UserId).NotEmpty();
            RuleFor(x => x.Role).NotEmpty().Must(r => r == "Student" || r == "Instructor" || r == "Admin");
        }
    }

    public sealed class Handler(IIdentityUnitOfWork unitOfWork) : IRequestHandler<Command, Result<bool>>
    {
        public async Task<Result<bool>> Handle(Command request, CancellationToken cancellationToken)
        {
            var user = await unitOfWork.Users.GetByIdAsync(request.UserId, cancellationToken);
            
            if (user == null)
            {
                return Result<bool>.Failure("User not found.");
            }

            user.ChangeRole(request.Role);
            
            unitOfWork.Users.Update(user);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<bool>.Success(true);
        }
    }
}
