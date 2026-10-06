using System;
using System.Threading;
using System.Threading.Tasks;
using Learning.Application.Common.Interfaces;
using Learning.Domain.Entities;
using Microsoft.EntityFrameworkCore.Storage;

namespace Learning.Persistence.Repositories;

public class LearningUnitOfWork(LearningDbContext context) : ILearningUnitOfWork
{
    private IRepository<Learning.Domain.Entities.Enrollment>? _enrollments;
    private IRepository<Learning.Domain.Entities.TestAttempt>? _testAttempts;

    public IRepository<Learning.Domain.Entities.Enrollment> Enrollments => _enrollments ??= new Repository<Learning.Domain.Entities.Enrollment>(context);
    public IRepository<Learning.Domain.Entities.TestAttempt> TestAttempts => _testAttempts ??= new Repository<Learning.Domain.Entities.TestAttempt>(context);

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await context.SaveChangesAsync(cancellationToken);
    }

    public async Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        return await context.Database.BeginTransactionAsync(cancellationToken);
    }

    public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (context.Database.CurrentTransaction != null)
        {
            await context.Database.CurrentTransaction.CommitAsync(cancellationToken);
        }
    }

    public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (context.Database.CurrentTransaction != null)
        {
            await context.Database.CurrentTransaction.RollbackAsync(cancellationToken);
        }
    }

    public IExecutionStrategy CreateExecutionStrategy()
    {
        return context.Database.CreateExecutionStrategy();
    }

    public async ValueTask DisposeAsync()
    {
        await context.DisposeAsync();
        GC.SuppressFinalize(this);
    }
}
