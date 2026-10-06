using System;
using System.Collections.Generic;
using Learning.Domain.Common;

namespace Learning.Domain.Entities;

public class Enrollment : BaseEntity
{
    public Guid StudentId { get; private set; }
    public Guid CourseId { get; private set; }
    public DateTime EnrollmentDate { get; private set; }
    public EnrollmentStatus Status { get; private set; }
    
    private readonly List<TestAttempt> _testAttempts = new();
    public IReadOnlyCollection<TestAttempt> TestAttempts => _testAttempts.AsReadOnly();

    private Enrollment() { } // EF Core

    public static Enrollment Create(Guid studentId, Guid courseId)
    {
        return new Enrollment
        {
            StudentId = studentId,
            CourseId = courseId,
            EnrollmentDate = DateTime.UtcNow,
            Status = EnrollmentStatus.Active
        };
    }

    public void AddTestAttempt(TestAttempt attempt)
    {
        _testAttempts.Add(attempt);
    }
}
