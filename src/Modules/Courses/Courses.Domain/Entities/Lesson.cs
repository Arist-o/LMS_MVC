using System;
using System.Collections.Generic;
using System.Text;
using Courses.Domain.Common;

namespace Courses.Domain.Entities
{
    public class Lesson : BaseEntity
    {
        public Guid CourseId { get; private set; }

        public Course Course { get; private set; }
        public Test Test { get; private set; }

        public string Title { get; private set; }

        public string VideoUrl { get; private set; }

        public int OrderIndex { get; private set; }

        public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; private set; } = DateTime.UtcNow;

        private Lesson()
        {

        }

        public Lesson(Guid coursesId, string title, string videoUrl, int orderIndex)
        {
            if (coursesId == Guid.Empty)
                throw new ArgumentException("CoursesId cannot be empty", nameof(coursesId));
            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentException("Title cannot be empty", nameof(title));
            if (string.IsNullOrWhiteSpace(videoUrl))
                throw new ArgumentException("VideoUrl cannot be empty", nameof(videoUrl));
            if (orderIndex < 0)
                throw new ArgumentException("OrderIndex cannot be negative", nameof(orderIndex));
            this.CourseId = coursesId;
            this.Title = title;
            this.VideoUrl = videoUrl;
            this.OrderIndex = orderIndex;
        }

        public void UpdatedAtTime(){
            this.UpdatedAt = DateTime.UtcNow;
        }

        public void UpdateTitle(string title)
        {
            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentException("Title cannot be empty", nameof(title));
            this.Title = title;
        }

        public void UpdateVideoUrl(string videoUrl)
        {
            if (string.IsNullOrWhiteSpace(videoUrl))
                throw new ArgumentException("VideoUrl cannot be empty", nameof(videoUrl));
            this.VideoUrl = videoUrl;
        }

        public void UpdateOrderIndex(int index)
        {
            if (index < 0)
                throw new ArgumentException("OrderIndex cannot be negative", nameof(index));
            this.OrderIndex = index;
        }
        public void MoveToCourse(Guid newCourseId)
        {
            if (newCourseId == Guid.Empty)
                throw new ArgumentException("CourseId cannot be empty", nameof(newCourseId));

            this.CourseId = newCourseId;
            
        }
    }
}
