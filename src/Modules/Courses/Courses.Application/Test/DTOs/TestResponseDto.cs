using System;
using System.Collections.Generic;

namespace Courses.Application.Test.DTOs;

public record TestResponseDto(Guid Id, Guid LessonId, string Title, int MaxScore, DateTime CreatedAt, DateTime UpdatedAt, IReadOnlyCollection<QuestionResponseDto> Questions = null);
