using System;
using Learning.Domain.Common;

namespace Learning.Domain.Entities;

public class TestAttempt : BaseEntity
{
    public Guid EnrollmentId { get; private set; }
    public Guid TestId { get; private set; }
    public int Score { get; private set; }
    public DateTime AttemptDate { get; private set; }

    private TestAttempt() { } // EF Core

    public static TestAttempt Create(Guid enrollmentId, Guid testId, int score)
    {
        return new TestAttempt
        {
            EnrollmentId = enrollmentId,
            TestId = testId,
            Score = score,
            AttemptDate = DateTime.UtcNow
        };
    }
}
