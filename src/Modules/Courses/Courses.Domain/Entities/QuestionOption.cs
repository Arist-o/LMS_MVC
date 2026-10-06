using System;
using Courses.Domain.Common;

namespace Courses.Domain.Entities
{
    public class QuestionOption : BaseEntity
    {
        public Guid QuestionId { get; private set; }
        public string Text { get; private set; }
        public bool IsCorrect { get; private set; }

        private QuestionOption()
        {
        }

        public QuestionOption(Guid questionId, string text, bool isCorrect)
        {
            if (questionId == Guid.Empty)
                throw new ArgumentException("QuestionId cannot be empty", nameof(questionId));
            if (string.IsNullOrWhiteSpace(text))
                throw new ArgumentException("Text cannot be empty", nameof(text));

            this.QuestionId = questionId;
            this.Text = text;
            this.IsCorrect = isCorrect;
        }

        public void Update(string text, bool isCorrect)
        {
            if (string.IsNullOrWhiteSpace(text))
                throw new ArgumentException("Text cannot be empty", nameof(text));

            this.Text = text;
            this.IsCorrect = isCorrect;
        }
    }
}
