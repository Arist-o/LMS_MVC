using System;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Learning.Application.Enrollment.Commands;
using Learning.Application.Enrollment.DTOs;
using Learning.Application.Enrollment.Queries;
using Learning.Application.TestAttempt.Commands;
using Learning.Application.TestAttempt.DTOs;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Courses.Integration;

namespace Learning.Presentation.Controllers;

[Route("enrollments")]
[Authorize(Roles = "Student")]
public class EnrollmentsController(IMediator mediator, ICourseIntegrationService courseIntegrationService) : Controller
{
    private Guid GetStudentId()
    {
        var idClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return Guid.TryParse(idClaim, out var id) ? id : Guid.Empty;
    }

    private async Task PopulateCoursesDropdown(CancellationToken ct)
    {
        var courses = await courseIntegrationService.GetCoursesAsync(ct);
        ViewBag.Courses = new SelectList(courses, "Id", "Title");
    }

    [HttpGet]
    public async Task<IActionResult> Index(CancellationToken ct)
    {
        var studentId = GetStudentId();
        if (studentId == Guid.Empty) return Unauthorized();

        var query = new GetEnrollments.Query(studentId);
        var result = await mediator.Send(query, ct);

        if (result.IsSuccess)
        {
            return View(result.Value);
        }
        
        return View(new List<EnrollmentResponseDto>());
    }

    [HttpGet("enroll")]
    public async Task<IActionResult> Enroll(CancellationToken ct)
    {
        await PopulateCoursesDropdown(ct);
        return View(new EnrollStudentDto(Guid.Empty));
    }

    [HttpPost("enroll")]
    public async Task<IActionResult> Enroll(EnrollStudentDto dto, CancellationToken ct)
    {
        var studentId = GetStudentId();
        if (studentId == Guid.Empty) return Unauthorized();

        if (!ModelState.IsValid)
        {
            await PopulateCoursesDropdown(ct);
            return View(dto);
        }

        var command = new EnrollStudent.Command(studentId, dto);
        var result = await mediator.Send(command, ct);

        if (result.IsSuccess)
        {
            return RedirectToAction(nameof(Index));
        }

        ModelState.AddModelError(string.Empty, result.Error);
        await PopulateCoursesDropdown(ct);
        return View(dto);
    }

    [Route("/Tests/Take/{testId:guid}")]
    [HttpGet]
    public async Task<IActionResult> TakeTest(Guid testId, CancellationToken ct)
    {
        var studentId = GetStudentId();
        if (studentId == Guid.Empty) return Unauthorized();

        var query = new Learning.Application.TestAttempt.Queries.GetTestAttemptForm.Query(testId);
        var result = await mediator.Send(query, ct);

        if (!result.IsSuccess)
            return NotFound(result.Error);

        return View(result.Value);
    }

    [Route("/Tests/Take/{testId:guid}")]
    [HttpPost]
    public async Task<IActionResult> TakeTest(Guid testId, SubmitTestAttemptDto dto, CancellationToken ct)
    {
        var studentId = GetStudentId();
        if (studentId == Guid.Empty) return Unauthorized();

        dto.TestId = testId;

        if (!ModelState.IsValid)
        {
            var query = new Learning.Application.TestAttempt.Queries.GetTestAttemptForm.Query(testId);
            var queryResult = await mediator.Send(query, ct);
            return View(queryResult.IsSuccess ? queryResult.Value : null);
        }

        var command = new SubmitTestAttempt.Command(studentId, dto);
        var result = await mediator.Send(command, ct);

        if (result.IsSuccess)
        {
            return RedirectToAction(nameof(Index));
        }

        ModelState.AddModelError(string.Empty, result.Error);
        var reloadQuery = new Learning.Application.TestAttempt.Queries.GetTestAttemptForm.Query(testId);
        var reloadResult = await mediator.Send(reloadQuery, ct);
        return View(reloadResult.IsSuccess ? reloadResult.Value : null);
    }
}
