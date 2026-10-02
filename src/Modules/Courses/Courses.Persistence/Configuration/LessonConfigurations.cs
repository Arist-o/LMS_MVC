using System;
using System.Collections.Generic;
using System.Text;
using Courses.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Courses.Persistence.Configuration
{
    public class LessonConfigurations : IEntityTypeConfiguration<Lesson>
    {
        public void Configure(EntityTypeBuilder<Lesson> builder)
        {
            builder.ToTable("Lessons");

            builder.HasKey(l => l.Id);

            builder.Property(l => l.CourseId)
                .IsRequired();

            builder.Property(l => l.Title)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(l => l.VideoUrl)
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(l => l.OrderIndex)
                .IsRequired();

            builder.HasOne(x => x.Course)
                .WithMany(x => x.Lessons)
                .HasForeignKey(x => x.CourseId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(l => l.Test)
                .WithOne(t => t.Lesson)
                .HasForeignKey<Test>(t => t.LessonId) 
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
