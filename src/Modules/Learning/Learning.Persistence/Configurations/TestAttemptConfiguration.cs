using Learning.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Learning.Persistence.Configurations;

public class TestAttemptConfiguration : IEntityTypeConfiguration<TestAttempt>
{
    public void Configure(EntityTypeBuilder<TestAttempt> builder)
    {
        builder.ToTable("TestAttempts", "learning");
        builder.HasKey(e => e.Id);
        
        // Soft FK
        builder.Property(e => e.TestId).IsRequired();
        
        // FK to Enrollment within same module
        builder.HasOne<Enrollment>()
               .WithMany(e => e.TestAttempts)
               .HasForeignKey(t => t.EnrollmentId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}
