using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace Courses.Application.Review.DTOs
{
    public sealed record CreateReviewDto(
        Guid CourseId,
        Guid StudentId,
        int Rating,
        string Comment,
        bool IsApproved
        );
}
