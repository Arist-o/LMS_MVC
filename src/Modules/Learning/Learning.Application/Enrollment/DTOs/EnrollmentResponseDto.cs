using System;
using System.Collections.Generic;
using Courses.Integration;

namespace Learning.Application.Enrollment.DTOs;

public record EnrollmentResponseDto(Guid Id, Guid CourseId, DateTime EnrollmentDate, string Status)
{
    public IReadOnlyCollection<TestLookupDto> AvailableTests { get; init; } = Array.Empty<TestLookupDto>();
}
