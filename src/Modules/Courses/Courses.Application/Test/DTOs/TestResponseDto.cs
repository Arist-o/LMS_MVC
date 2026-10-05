using System;

namespace Courses.Application.Test.DTOs;

public record TestResponseDto(Guid Id, Guid LessonId, string Title, int MaxScore, DateTime CreatedAt, DateTime UpdatedAt);
