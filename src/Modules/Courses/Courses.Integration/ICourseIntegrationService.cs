namespace Courses.Integration;

public interface ICourseIntegrationService
{
    Task<bool> CourseExistsAsync(Guid courseId, CancellationToken cancellationToken = default);
    Task<bool> TestExistsAsync(Guid testId, CancellationToken cancellationToken = default);
    Task<IEnumerable<CourseLookupDto>> GetCoursesAsync(CancellationToken cancellationToken = default);
}
