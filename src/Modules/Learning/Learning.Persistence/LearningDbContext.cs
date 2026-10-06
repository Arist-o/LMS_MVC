using Learning.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Learning.Persistence;

public class LearningDbContext : DbContext
{
    public LearningDbContext(DbContextOptions<LearningDbContext> options) : base(options)
    {
    }

    public DbSet<Enrollment> Enrollments { get; set; } = null!;
    public DbSet<TestAttempt> TestAttempts { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("learning");
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(LearningDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
