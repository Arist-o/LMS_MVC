using System;
using System.Collections.Generic;
using System.Text;
using Courses.Application.Common.Interfaces;
using Courses.Domain.Entities;
using Courses.Persistence.Context;
using Microsoft.EntityFrameworkCore.Storage;

namespace Courses.Persistence.Repositories
{
    public sealed class UnitOfWork : IUnitOfWork
    {
        private readonly CoursesDbContext _context;

        private IDbContextTransaction? _currentTransaction;

        private IRepository<Course>? _coursesRepository;

        private IRepository<Lesson>? _lessonsRepository;

        private IRepository<Test>? _testsRepository;

        private IRepository<Review>? _reviewsRepository;

        public UnitOfWork(CoursesDbContext context)
        {
            _context = context;
        }

        public IRepository<Course> Courses 
            => _coursesRepository ??= new Repository<Course>(_context);

        public IRepository<Lesson> Lessons
            => _lessonsRepository ??= new Repository<Lesson>(_context);

        public IRepository<Test> Tests
            => _testsRepository ??= new Repository<Test>(_context);

        public IRepository<Review> Reviews
            => _reviewsRepository ??= new Repository<Review>(_context);

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
            => await _context.SaveChangesAsync(cancellationToken);

        public async Task<IDbContextTransaction> BeginTransactionAsync(
            CancellationToken cancellationToken = default)
        {
            if (_currentTransaction is not null)
                throw new InvalidOperationException(
                    "Transaction is already active. Nested transactions are not supported.");

            _currentTransaction =
                await _context.Database.BeginTransactionAsync(cancellationToken);

            return _currentTransaction;
        }

        public async Task CommitTransactionAsync(
            CancellationToken cancellationToken = default)
        {
            if (_currentTransaction is null)
                throw new InvalidOperationException(
                    "Немає активної транзакції для коміту.");

            try
            {
                await _currentTransaction.CommitAsync(cancellationToken);
            }
            finally
            {
                await _currentTransaction.DisposeAsync();
                _currentTransaction = null;
            }
        }
        public async Task RollbackTransactionAsync(
            CancellationToken cancellationToken = default)
        {
            if (_currentTransaction is null) return;

            try
            {
                await _currentTransaction.RollbackAsync(cancellationToken);
            }
            finally
            {
                await _currentTransaction.DisposeAsync();
                _currentTransaction = null;
            }
        }

        public IExecutionStrategy CreateExecutionStrategy()
            => _context.Database.CreateExecutionStrategy();


        public async ValueTask DisposeAsync()
        {
            if (_currentTransaction is not null)
                await _currentTransaction.DisposeAsync();

            await _context.DisposeAsync();
        }
    }
}
