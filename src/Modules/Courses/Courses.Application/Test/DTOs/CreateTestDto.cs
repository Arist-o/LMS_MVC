using System;

namespace Courses.Application.Test.DTOs;

public record CreateTestDto(Guid LessonId, string Title, int MaxScore);
