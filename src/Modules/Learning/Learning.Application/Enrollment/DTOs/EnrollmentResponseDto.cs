using System;

namespace Learning.Application.Enrollment.DTOs;

public record EnrollmentResponseDto(Guid Id, Guid CourseId, DateTime EnrollmentDate, string Status);
