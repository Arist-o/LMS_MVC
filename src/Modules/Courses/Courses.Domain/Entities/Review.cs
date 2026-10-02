using System;
using System.Collections.Generic;
using System.Text;
using Courses.Domain.Common;

namespace Courses.Domain.Entities
{
    public class Review : BaseEntity
    {
        public Guid CourseId { get; private set; }

        public Course Course { get; private set; }

        public Guid StudentId { get; private set; }

        public int Rating { get; private set; }

        public string Comment { get; private set; }

        public bool IsApproved { get; private set; }

        public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; private set; } = DateTime.UtcNow;

        private Review()
        {
        }

        public Review(Guid courseId , Guid studentId, int rating,string comment, bool isApproved)
        {
            if (courseId == Guid.Empty)
                throw new ArgumentException("CourseId cannot be empty", nameof(courseId));

            if (studentId == Guid.Empty)
                throw new ArgumentException("StudentId cannot be empty",nameof(studentId));

            if (rating < 0)
                throw new ArgumentException("Rating cannot be negative", nameof(rating));

            if (string.IsNullOrWhiteSpace(comment))
                throw new ArgumentException("Comment cannot be empty", nameof(comment));

            this.CourseId = courseId;
            this.StudentId = studentId;
            this.Rating = rating;
            this.Comment = comment;
            this.IsApproved = isApproved;
        }

        public void UpdatedAtTime()
        {
            this.UpdatedAt = DateTime.UtcNow;
        }

        public void UpdateCourseId(Guid courseId)
        { 
            if (courseId == Guid.Empty)
                throw new ArgumentException("CourseId cannot be empty", nameof(courseId));
            this.CourseId = courseId;
        }

        public void UpdateStudentId(Guid studentId)
        {
            if (studentId == Guid.Empty)
                throw new ArgumentException("StudentId cannot be empty", nameof(studentId));
            this.StudentId = studentId;
        }

        public void UpdateRating(int rating)
        {
            if (rating < 0)
                throw new ArgumentException("Rating cannot be negative", nameof(rating));
            this.Rating = rating;
        }

        public void UpdateIsApproved(bool isApproved)
        {
            this.IsApproved = isApproved;
        }

        public void UpdateComment(string comment)
        {
            if(string.IsNullOrWhiteSpace(comment))
                throw new ArgumentException("Comment cannot be empty", nameof(comment));
            this.Comment = comment;

        }
    }

}
