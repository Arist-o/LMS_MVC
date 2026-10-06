using System;

namespace Learning.Application.TestAttempt.DTOs;

public record TestAttemptResponseDto(Guid Id, Guid EnrollmentId, Guid TestId, int Score, DateTime AttemptDate);
