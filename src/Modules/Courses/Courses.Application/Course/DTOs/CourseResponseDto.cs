using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace Courses.Application.Course.DTOs
{
    public sealed record CourseResponseDto(
        Guid Id,
        Guid InstructorId,
        string Title,
        string Description,
        bool IsPublished,
        DateTime CreatedAt
    );
}
