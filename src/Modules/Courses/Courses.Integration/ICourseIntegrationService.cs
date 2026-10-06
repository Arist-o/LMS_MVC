namespace Courses.Integration;

public record TestAttemptOptionDto(Guid Id, string Text);
public record TestAttemptQuestionDto(Guid Id, string Text, int Points, IReadOnlyCollection<TestAttemptOptionDto> Options);
public record TestAttemptFormDto(Guid TestId, string TestTitle, IReadOnlyCollection<TestAttemptQuestionDto> Questions);
public record TestEvaluationResultDto(Guid CourseId, int Score);
public record TestLookupDto(Guid Id, string Title);

public interface ICourseIntegrationService
{
    Task<bool> CourseExistsAsync(Guid courseId, CancellationToken cancellationToken = default);
    Task<bool> TestExistsAsync(Guid testId, CancellationToken cancellationToken = default);
    Task<IEnumerable<CourseLookupDto>> GetCoursesAsync(CancellationToken cancellationToken = default);
    
    Task<Dictionary<Guid, IEnumerable<TestLookupDto>>> GetTestsForCoursesAsync(IEnumerable<Guid> courseIds, CancellationToken cancellationToken = default);
    Task<TestAttemptFormDto> GetTestAttemptDataAsync(Guid testId, CancellationToken cancellationToken = default);
    Task<TestEvaluationResultDto> EvaluateTestAsync(Guid testId, Dictionary<Guid, Guid> chosenAnswers, CancellationToken cancellationToken = default);
}
