using System;

namespace Courses.Application.Review.DTOs;

public sealed record ReviewResponseDto(
    Guid Id,
    Guid CourseId,
    Guid StudentId,
    int Rating,
    string Comment,
    bool IsApproved,
    DateTime CreatedAt
);
