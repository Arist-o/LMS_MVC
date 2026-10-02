using System;
using System.Collections.Generic;
using System.Text;
using Courses.Domain.Common;

namespace Courses.Domain.Entities
{
    public class Test : BaseEntity
    {
        public Guid LessonId { get; private set; }

        public Lesson Lesson { get; private set; }

        public string Title { get; private set; }

        public int MaxScore { get; private set; }

        public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; private set; } = DateTime.UtcNow;


        private Test()
        {
        }

        public Test(Guid lessonId, string title, int maxScore)
        {
            if (lessonId == Guid.Empty)
                throw new ArgumentException("LessonId cannot be empty", nameof(lessonId));
            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentException("Title cannot be empty", nameof(title));
            if (maxScore < 0)
                throw new ArgumentException("MaxScore cannot be negative", nameof(maxScore));
            this.LessonId = lessonId;
            this.Title = title;
            this.MaxScore = maxScore;
        }

        public void UpdatedAtTime()
        {
            this.UpdatedAt = DateTime.UtcNow;
        }

        public void UpdateLessonId(Guid lessonId)
        {
            if (lessonId == Guid.Empty)
                throw new ArgumentException("LessonId cannot be empty", nameof(lessonId));
            this.LessonId = lessonId;
        }

        public void UpdateTitle(string title)
        {
            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentException("Title cannot be empty", nameof(title));
            this.Title = title;
        }

        public void UpdateMaxScore(int maxScore)
        {
            if (maxScore < 0)
                throw new ArgumentException("MaxScore cannot be negative", nameof(maxScore));
            this.MaxScore = maxScore;
        }
    }
}
