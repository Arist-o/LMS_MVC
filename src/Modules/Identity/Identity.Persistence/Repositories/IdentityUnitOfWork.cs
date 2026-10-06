using Identity.Application.Common.Interfaces;
using Identity.Domain.Entities;
using Microsoft.EntityFrameworkCore.Storage;
using System.Threading;
using System.Threading.Tasks;

namespace Identity.Persistence.Repositories;

public class IdentityUnitOfWork(IdentityDbContext context) : IIdentityUnitOfWork
{
    private IRepository<User>? _users;

    public IRepository<User> Users => _users ??= new Repository<User>(context);

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return context.SaveChangesAsync(cancellationToken);
    }

    public Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        return context.Database.BeginTransactionAsync(cancellationToken);
    }

    public Task CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
        return context.Database.CommitTransactionAsync(cancellationToken);
    }

    public Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
    {
        return context.Database.RollbackTransactionAsync(cancellationToken);
    }

    public IExecutionStrategy CreateExecutionStrategy()
    {
        return context.Database.CreateExecutionStrategy();
    }

    public ValueTask DisposeAsync()
    {
        return context.DisposeAsync();
    }
}
