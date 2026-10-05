using System;
using System.Collections.Generic;
using System.Text;
using Courses.Domain.Entities;
using Microsoft.EntityFrameworkCore.Storage;
using DomainReview = Courses.Domain.Entities.Review;
namespace Courses.Application.Common.Interfaces
{
    public interface IUnitOfWork : IAsyncDisposable
    {
        IRepository<Domain.Entities.Course> Courses { get; }

        IRepository<Lesson> Lessons { get; }

        IRepository<Test> Tests { get; }

        IRepository<DomainReview> Reviews { get; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

        Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default);

        Task CommitTransactionAsync(CancellationToken cancellationToken = default);

        Task RollbackTransactionAsync(CancellationToken cancellationToken = default);

        IExecutionStrategy CreateExecutionStrategy();
    }
}
