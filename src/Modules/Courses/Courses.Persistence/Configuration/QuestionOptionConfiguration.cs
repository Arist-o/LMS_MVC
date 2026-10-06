using Courses.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Courses.Persistence.Configuration
{
    public class QuestionOptionConfiguration : IEntityTypeConfiguration<QuestionOption>
    {
        public void Configure(EntityTypeBuilder<QuestionOption> builder)
        {
            builder.ToTable("QuestionOptions");

            builder.HasKey(o => o.Id);

            builder.Property(o => o.Text)
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(o => o.IsCorrect)
                .IsRequired();

            builder.HasOne<Question>()
                .WithMany(q => q.Options)
                .HasForeignKey(o => o.QuestionId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
