using Learning.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Learning.Persistence.Configurations;

public class EnrollmentConfiguration : IEntityTypeConfiguration<Enrollment>
{
    public void Configure(EntityTypeBuilder<Enrollment> builder)
    {
        builder.ToTable("Enrollments", "learning");
        builder.HasKey(e => e.Id);
        
        // Soft FKs
        builder.Property(e => e.StudentId).IsRequired();
        builder.Property(e => e.CourseId).IsRequired();
        
        builder.Property(e => e.Status)
               .HasConversion<string>()
               .HasMaxLength(50)
               .IsRequired();
    }
}
