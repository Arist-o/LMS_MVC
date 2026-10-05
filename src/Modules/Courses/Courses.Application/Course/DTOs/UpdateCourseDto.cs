using System;

namespace Courses.Application.Course.DTOs;

public sealed record UpdateCourseDto(
    Guid Id,
    string Title,
    string Description,
    bool IsPublished
);
