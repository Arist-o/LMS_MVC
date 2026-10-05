using System;

namespace Courses.Application.Lesson.DTOs;

public record CreateLessonDto(
    Guid CourseId,
    string Title,
    string VideoUrl,
    int OrderIndex
);
