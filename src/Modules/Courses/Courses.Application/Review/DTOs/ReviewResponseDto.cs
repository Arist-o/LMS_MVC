using System;
using System.Collections.Generic;
using System.Text;

namespace Courses.Application.Review.DTOs
{
    public sealed record ReviewResponseDto(
        Guid CourseId,
        Guid StudentId,
        int Rating,
        string Comment,
        bool IsApproved,
        DateTime CreatedAt
        );
}
