using System;
using System.Threading;
using System.Threading.Tasks;
using Learning.Domain.Entities;
using Microsoft.EntityFrameworkCore.Storage;

namespace Learning.Application.Common.Interfaces;

public interface ILearningUnitOfWork : IAsyncDisposable
{
    IRepository<Learning.Domain.Entities.Enrollment> Enrollments { get; }
    IRepository<Learning.Domain.Entities.TestAttempt> TestAttempts { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

    Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default);

    Task CommitTransactionAsync(CancellationToken cancellationToken = default);

    Task RollbackTransactionAsync(CancellationToken cancellationToken = default);

    IExecutionStrategy CreateExecutionStrategy();
}
