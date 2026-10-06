using System;

namespace Courses.Application.Lesson.DTOs;

public record GetLessonsDto
{
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 10;
}
