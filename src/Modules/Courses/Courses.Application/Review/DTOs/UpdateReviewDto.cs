using System;

namespace Courses.Application.Review.DTOs;

public sealed record UpdateReviewDto(
    Guid Id,
    int Rating,
    string Comment,
    bool IsApproved
);
