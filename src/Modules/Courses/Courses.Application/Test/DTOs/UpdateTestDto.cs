using System;

namespace Courses.Application.Test.DTOs;

public record UpdateTestDto(Guid Id, Guid LessonId, string Title, int MaxScore);
