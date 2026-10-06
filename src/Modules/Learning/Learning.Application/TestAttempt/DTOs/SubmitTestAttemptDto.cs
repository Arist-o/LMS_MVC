using System;

namespace Learning.Application.TestAttempt.DTOs;

public record SubmitTestAttemptDto(Guid CourseId, Guid TestId, int Score);
