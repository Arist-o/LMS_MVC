using Courses.Application.Common.Interfaces;
using Courses.Integration;
using Microsoft.EntityFrameworkCore;

namespace Courses.Application.Services;

public class CourseIntegrationService(IUnitOfWork unitOfWork) : ICourseIntegrationService
{
    public async Task<bool> CourseExistsAsync(Guid courseId, CancellationToken cancellationToken = default)
    {
        return await unitOfWork.Courses.Query().AnyAsync(c => c.Id == courseId, cancellationToken);
    }

    public async Task<bool> TestExistsAsync(Guid testId, CancellationToken cancellationToken = default)
    {
        return await unitOfWork.Tests.Query().AnyAsync(t => t.Id == testId, cancellationToken);
    }

    public async Task<IEnumerable<CourseLookupDto>> GetCoursesAsync(CancellationToken cancellationToken = default)
    {
        return await unitOfWork.Courses.Query()
            .Select(c => new CourseLookupDto(c.Id, c.Title))
            .ToListAsync(cancellationToken);
    }
}
