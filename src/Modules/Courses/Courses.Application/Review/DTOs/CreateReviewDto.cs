using System;

namespace Courses.Application.Review.DTOs;

public sealed record CreateReviewDto(
    Guid CourseId,
    Guid StudentId,
    int Rating,
    string Comment,
    bool IsApproved
);
