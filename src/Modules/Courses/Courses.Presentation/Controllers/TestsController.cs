using System;
using System.Threading;
using System.Threading.Tasks;
using Courses.Application.Common.Models;
using Courses.Application.Test.Commands;
using Courses.Application.Test.DTOs;
using Courses.Application.Test.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Courses.Application.Lesson.Queries;

namespace Courses.Presentation.Controllers;

[Route("tests")]
public class TestsController(IMediator mediator) : Controller
{
    private async Task PopulateLessonsDropdown(CancellationToken ct)
    {
        var result = await mediator.Send(new GetLessonsLookup.Query(), ct);
        if (result.IsSuccess)
        {
            ViewBag.Lessons = new SelectList(result.Value, "Id", "Name");
        }
    }

    [HttpGet]
    public async Task<IActionResult> Index(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? search = null,
        [FromQuery] string? sortBy = null,
        CancellationToken ct = default)
    {
        var query = new GetTests.Query(page, pageSize, search, sortBy);
        var result = await mediator.Send(query, ct);

        if (!result.IsSuccess)
        {
            ModelState.AddModelError(string.Empty, result.Error);
            return View(new PagedList<TestResponseDto>([], 0, page, pageSize));
        }

        return View(result.Value);
    }

    [HttpGet("create")]
    public async Task<IActionResult> Create(CancellationToken ct)
    {
        await PopulateLessonsDropdown(ct);
        return View(new CreateTestDto(Guid.Empty, string.Empty, 0));
    }

    [HttpPost("create")]
    public async Task<IActionResult> Create(CreateTestDto dto, CancellationToken ct)
    {
        if (!ModelState.IsValid)
        {
            await PopulateLessonsDropdown(ct);
            return View(dto);
        }

        var command = new CreateTest.Command(dto);
        var result = await mediator.Send(command, ct);

        if (result.IsSuccess)
            return RedirectToAction(nameof(Index));

        ModelState.AddModelError(string.Empty, result.Error);
        await PopulateLessonsDropdown(ct);
        return View(dto);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> Details(Guid id, CancellationToken ct)
    {
        var result = await mediator.Send(new GetTestById.Query(id), ct);
        if (!result.IsSuccess) return NotFound();
        return View(result.Value);
    }

    [HttpGet("{id:guid}/edit")]
    public async Task<IActionResult> Edit(Guid id, CancellationToken ct)
    {
        var result = await mediator.Send(new GetTestById.Query(id), ct);
        if (!result.IsSuccess) return NotFound();

        await PopulateLessonsDropdown(ct);
        var dto = new UpdateTestDto(result.Value.Id, result.Value.LessonId, result.Value.Title, result.Value.MaxScore);
        return View(dto);
    }

    [HttpPost("{id:guid}/edit")]
    public async Task<IActionResult> Edit(Guid id, UpdateTestDto dto, CancellationToken ct)
    {
        if (id != dto.Id) return BadRequest();

        if (!ModelState.IsValid)
        {
            await PopulateLessonsDropdown(ct);
            return View(dto);
        }

        var command = new UpdateTest.Command(dto);
        var result = await mediator.Send(command, ct);

        if (result.IsSuccess)
            return RedirectToAction(nameof(Index));

        ModelState.AddModelError(string.Empty, result.Error);
        await PopulateLessonsDropdown(ct);
        return View(dto);
    }

    [HttpGet("{id:guid}/delete")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        var result = await mediator.Send(new GetTestById.Query(id), ct);
        if (!result.IsSuccess) return NotFound();
        return View(result.Value);
    }

    [HttpPost("{id:guid}/delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(Guid id, CancellationToken ct)
    {
        var command = new DeleteTest.Command(id);
        var result = await mediator.Send(command, ct);

        if (result.IsSuccess)
            return RedirectToAction(nameof(Index));

        ModelState.AddModelError(string.Empty, result.Error);
        var test = await mediator.Send(new GetTestById.Query(id), ct);
        return View("Delete", test.IsSuccess ? test.Value : null);
    }
}
