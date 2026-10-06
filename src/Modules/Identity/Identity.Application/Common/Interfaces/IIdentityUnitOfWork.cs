using System;
using System.Threading;
using System.Threading.Tasks;
using Identity.Domain.Entities;
using Microsoft.EntityFrameworkCore.Storage;

namespace Identity.Application.Common.Interfaces;

public interface IIdentityUnitOfWork : IAsyncDisposable
{
    IRepository<User> Users { get; }
    
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

    Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default);

    Task CommitTransactionAsync(CancellationToken cancellationToken = default);

    Task RollbackTransactionAsync(CancellationToken cancellationToken = default);

    IExecutionStrategy CreateExecutionStrategy();
}
