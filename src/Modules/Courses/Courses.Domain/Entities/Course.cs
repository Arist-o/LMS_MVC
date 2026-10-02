using System;
using System.Collections.Generic;
using System.Text;
using Courses.Domain.Common;

namespace Courses.Domain.Entities
{
    public class Course : BaseEntity
    {
        public Guid InstructorId { get; private set; }

        public string Title { get; private set; }

        public string Description { get; private set; }

        public bool IsPublished { get; private set; }

        private readonly List<Lesson> _lessons = new();

        public IReadOnlyCollection<Lesson> Lessons => _lessons.AsReadOnly();

        private readonly List<Review> _reviews = new();

        public IReadOnlyCollection<Review> Reviews => _reviews.AsReadOnly();

        public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; private set; } = DateTime.UtcNow;

        private Course()
        {

        }

        public void UpdateAtTime()
        {
            this.UpdatedAt = DateTime.UtcNow;
        }

        public void AddReview(Review review)
        {
            if (review == null) throw new ArgumentNullException(nameof(review));
            _reviews.Add(review);
        }

        public void AddLesson(Lesson lesson)
        {
            if (lesson == null) throw new ArgumentNullException(nameof(lesson));
            _lessons.Add(lesson);
        }
        public Course(Guid instructorId, string title, string description, bool isPublished)
        {
            if (instructorId == Guid.Empty)
                throw new ArgumentException("InstructorId cannot be empty", nameof(instructorId));

            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentException("Title cannot be empty", nameof(title));

            if (string.IsNullOrWhiteSpace(description))
                throw new ArgumentException("Description cannot be empty", nameof(description));


            this.InstructorId = instructorId;
            this.Title = title;
            this.Description = description;
            this.IsPublished = isPublished;
        }


        public void UpdateInstructor(Guid instructorId)
        {
            if (instructorId == Guid.Empty)
                throw new ArgumentException("InstructorId cannot be empty", nameof(instructorId));

            this.InstructorId = instructorId;
        }

        public void UpdateTitle(string title)
        {
            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentException("Title cannot be empty", nameof(title));

            this.Title = title;
        }

        public void UpdateDescription(string description) {
            if(string.IsNullOrWhiteSpace(description))
                throw new ArgumentException("Description cannot be empty", nameof(description));

            this.Description = description;
        }

        public void UpdateIsPublished(bool isPublished)
        {
            this.IsPublished = isPublished;
        }

    }
}
