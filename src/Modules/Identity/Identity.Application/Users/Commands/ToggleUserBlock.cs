using Identity.Application.Common.Interfaces;
using Identity.Application.Common.Models;
using MediatR;

namespace Identity.Application.Users.Commands;

public static class ToggleUserBlock
{
    public sealed record Command(Guid UserId) : IRequest<Result<bool>>;

    public sealed class Handler(IIdentityUnitOfWork unitOfWork) : IRequestHandler<Command, Result<bool>>
    {
        public async Task<Result<bool>> Handle(Command request, CancellationToken cancellationToken)
        {
            var user = await unitOfWork.Users.GetByIdAsync(request.UserId, cancellationToken);
            
            if (user == null)
            {
                return Result<bool>.Failure("User not found.");
            }

            if (user.IsBlocked)
            {
                user.Unblock();
            }
            else
            {
                user.Block();
            }

            unitOfWork.Users.Update(user);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<bool>.Success(user.IsBlocked);
        }
    }
}
