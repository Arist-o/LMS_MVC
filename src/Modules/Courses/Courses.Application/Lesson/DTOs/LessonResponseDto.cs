using System;

namespace Courses.Application.Lesson.DTOs;

public record LessonResponseDto(
    Guid Id,
    Guid CourseId,
    string CourseTitle,
    string Title,
    string VideoUrl,
    int OrderIndex,
    DateTime CreatedAt
);
