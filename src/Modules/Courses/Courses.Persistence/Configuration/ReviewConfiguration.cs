using System;
using System.Collections.Generic;
using System.Text;
using Courses.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Courses.Persistence.Configuration
{
    public class ReviewConfiguration : IEntityTypeConfiguration<Review>
    {
        public void Configure(EntityTypeBuilder<Review> builder)
        {
            builder.ToTable("Reviews");

            builder.HasKey(r => r.Id);

            builder.Property(r => r.StudentId)
                .IsRequired();

            builder.Property(r => r.CourseId)
                .IsRequired();

            builder.Property(r => r.Rating)
                .IsRequired()
                .HasColumnType("int");

            builder.Property(r => r.Comment)
                .HasMaxLength(1000);
        }
    }
}
