using System;
using System.Threading;
using System.Threading.Tasks;
using Courses.Application.Test.Commands;
using Courses.Application.Test.DTOs;
using Courses.Application.Test.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Courses.Presentation.Controllers;

[Route("tests/{testId:guid}/questions")]
public class QuestionsController(IMediator mediator) : Controller
{
    [Authorize(Roles = "Instructor,Admin")]
    [HttpGet("create")]
    public async Task<IActionResult> Create(Guid testId, CancellationToken ct)
    {
        var testResult = await mediator.Send(new GetTestById.Query(testId), ct);
        if (!testResult.IsSuccess)
        {
            return NotFound();
        }

        ViewBag.TestTitle = testResult.Value.Title;
        return View(new AddQuestionToTestDto(testId, string.Empty, 1, []));
    }

    [Authorize(Roles = "Instructor,Admin")]
    [HttpPost("create")]
    public async Task<IActionResult> Create(Guid testId, AddQuestionToTestDto dto, CancellationToken ct)
    {
        if (testId != dto.TestId)
        {
            return BadRequest();
        }

        if (!ModelState.IsValid)
        {
            return View(dto);
        }

        var command = new AddQuestionToTest.Command(dto);
        var result = await mediator.Send(command, ct);

        if (result.IsSuccess)
        {
            return RedirectToAction("Details", "Tests", new { id = testId });
        }

        ModelState.AddModelError(string.Empty, result.Error);
        return View(dto);
    }
}
