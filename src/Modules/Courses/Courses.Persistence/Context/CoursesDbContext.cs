using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using Courses.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Courses.Persistence.Context
{
    public class CoursesDbContext(DbContextOptions<CoursesDbContext> options) : DbContext(options)
    {
        public DbSet<Course> Courses => Set<Course>();
        public DbSet<Test> Tests => Set<Test>();
        public DbSet<Lesson> Lessons => Set<Lesson>();

        public DbSet<Question> Questions => Set<Question>();
        public DbSet<QuestionOption> QuestionOptions => Set<QuestionOption>();

        public DbSet<Review> Reviews => Set<Review>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(CoursesDbContext).Assembly);
        }

        public override Task<int> SaveChangesAsync(
            CancellationToken cancellationToken = default)
        {
            foreach (var entry in ChangeTracker.Entries())
            {
                if (entry.Entity is Course course
                    && entry.State == EntityState.Modified)
                {
                    course.UpdateAtTime();
                }

                if (entry.Entity is Test test
                    && entry.State == EntityState.Modified)
                {
                    test.UpdatedAtTime();
                }

                if(entry.Entity is Lesson lesson 
                   && entry.State == EntityState.Modified)
                {
                    lesson.UpdatedAtTime();
                }

                if (entry.Entity is Review review
                    && entry.State == EntityState.Modified)
                {
                    review.UpdatedAtTime();
                }
            }

            return base.SaveChangesAsync(cancellationToken);
        }
    }
}
