using System;

namespace Courses.Application.Lesson.DTOs;

public record UpdateLessonDto(
    Guid Id,
    Guid CourseId,
    string Title,
    string VideoUrl,
    int OrderIndex
);
