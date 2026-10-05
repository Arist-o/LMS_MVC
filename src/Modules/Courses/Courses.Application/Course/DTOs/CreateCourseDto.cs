using System;
using System.Collections.Generic;
using System.Text;

namespace Courses.Application.Course.DTOs
{
    public sealed record class CreateCourseDto(
            Guid InstructorId,
            string Title,
            string Description,
            bool IsPublished
        );
}
