using System;
using System.Collections.Generic;
using Courses.Domain.Common;

namespace Courses.Domain.Entities
{
    public class Question : BaseEntity
    {
        public Guid TestId { get; private set; }
        public string Text { get; private set; }
        public int Points { get; private set; }

        private readonly List<QuestionOption> _options = [];
        public IReadOnlyCollection<QuestionOption> Options => _options.AsReadOnly();

        private Question()
        {
        }

        public Question(Guid testId, string text, int points)
        {
            if (testId == Guid.Empty)
                throw new ArgumentException("TestId cannot be empty", nameof(testId));
            if (string.IsNullOrWhiteSpace(text))
                throw new ArgumentException("Text cannot be empty", nameof(text));
            if (points < 0)
                throw new ArgumentException("Points cannot be negative", nameof(points));

            this.TestId = testId;
            this.Text = text;
            this.Points = points;
        }

        public void AddOption(QuestionOption option)
        {
            if (option == null) throw new ArgumentNullException(nameof(option));
            _options.Add(option);
        }

        public void Update(string text, int points)
        {
            if (string.IsNullOrWhiteSpace(text))
                throw new ArgumentException("Text cannot be empty", nameof(text));
            if (points < 0)
                throw new ArgumentException("Points cannot be negative", nameof(points));

            this.Text = text;
            this.Points = points;
        }
    }
}
