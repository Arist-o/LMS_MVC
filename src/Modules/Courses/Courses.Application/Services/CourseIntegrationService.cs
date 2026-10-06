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

    public async Task<Dictionary<Guid, IEnumerable<TestLookupDto>>> GetTestsForCoursesAsync(IEnumerable<Guid> courseIds, CancellationToken cancellationToken = default)
    {
        var tests = await unitOfWork.Tests.Query()
            .Include(t => t.Lesson)
            .Where(t => courseIds.Contains(t.Lesson.CourseId))
            .Select(t => new { t.Lesson.CourseId, t.Id, t.Title })
            .ToListAsync(cancellationToken);

        return tests.GroupBy(t => t.CourseId)
            .ToDictionary(
                g => g.Key, 
                g => g.Select(x => new TestLookupDto(x.Id, x.Title))
            );
    }

    public async Task<TestAttemptFormDto> GetTestAttemptDataAsync(Guid testId, CancellationToken cancellationToken = default)
    {
        var test = await unitOfWork.Tests.Query()
            .Include(t => t.Questions)
                .ThenInclude(q => q.Options)
            .FirstOrDefaultAsync(t => t.Id == testId, cancellationToken);

        if (test == null) throw new Exception($"Test with ID {testId} not found.");

        var questions = test.Questions.Select(q => new TestAttemptQuestionDto(
            q.Id,
            q.Text,
            q.Points,
            q.Options.Select(o => new TestAttemptOptionDto(o.Id, o.Text)).ToList()
        )).ToList();

        return new TestAttemptFormDto(test.Id, test.Title, questions);
    }

    public async Task<TestEvaluationResultDto> EvaluateTestAsync(Guid testId, Dictionary<Guid, Guid> chosenAnswers, CancellationToken cancellationToken = default)
    {
        var test = await unitOfWork.Tests.Query()
            .Include(t => t.Lesson)
            .Include(t => t.Questions)
                .ThenInclude(q => q.Options)
            .FirstOrDefaultAsync(t => t.Id == testId, cancellationToken);

        if (test == null) throw new Exception($"Test with ID {testId} not found.");

        int score = 0;
        foreach (var question in test.Questions)
        {
            if (chosenAnswers != null && chosenAnswers.TryGetValue(question.Id, out var chosenOptionId))
            {
                var option = question.Options.FirstOrDefault(o => o.Id == chosenOptionId);
                if (option != null && option.IsCorrect)
                {
                    score += question.Points;
                }
            }
        }

        return new TestEvaluationResultDto(test.Lesson.CourseId, score);
    }
}
