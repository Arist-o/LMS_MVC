using System;
using System.Threading;
using System.Threading.Tasks;
using Courses.Application.Lesson.Commands;
using Courses.Application.Lesson.DTOs;
using Courses.Application.Lesson.Queries;
using Courses.Application.Course.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Courses.Presentation.Controllers;

[Route("lessons")]
public class LessonsController(IMediator mediator) : Controller
{
    private async Task PopulateCoursesDropdown(CancellationToken ct)
    {
        var result = await mediator.Send(new GetCoursesLookup.Query(), ct);
        if (result.IsSuccess)
        {
            ViewBag.Courses = new SelectList(result.Value, "Id", "Name");
        }
    }

    [HttpGet]
    public async Task<IActionResult> Index([FromQuery] int page = 1, [FromQuery] int pageSize = 10, CancellationToken ct = default)
    {
        var result = await mediator.Send(new GetLessons.Query(page, pageSize), ct);
        if (!result.IsSuccess)
        {
            return BadRequest(result.Error);
        }
        return View(result.Value);
    }

    [Authorize(Roles = "Instructor,Admin")]
    [HttpGet("create")]
    public async Task<IActionResult> Create(CancellationToken ct)
    {
        await PopulateCoursesDropdown(ct);
        return View();
    }

    [Authorize(Roles = "Instructor,Admin")]
    [HttpPost("create")]
    public async Task<IActionResult> Create(CreateLessonDto dto, CancellationToken ct)
    {
        if (!ModelState.IsValid)
        {
            await PopulateCoursesDropdown(ct);
            return View(dto);
        }

        var result = await mediator.Send(new CreateLesson.Command(dto), ct);
        
        if (result.IsSuccess)
        {
            return RedirectToAction(nameof(Index));
        }

        ModelState.AddModelError(string.Empty, result.Error);
        await PopulateCoursesDropdown(ct);
        return View(dto);
    }

    [Authorize(Roles = "Instructor,Admin")]
    [HttpGet("edit/{id:guid}")]
    public async Task<IActionResult> Edit(Guid id, CancellationToken ct)
    {
        var result = await mediator.Send(new GetLessonById.Query(id), ct);
        if (!result.IsSuccess)
        {
            return NotFound();
        }

        await PopulateCoursesDropdown(ct);
        var dto = new UpdateLessonDto(
            result.Value.Id,
            result.Value.CourseId,
            result.Value.Title,
            result.Value.VideoUrl,
            result.Value.OrderIndex
        );

        return View(dto);
    }

    [Authorize(Roles = "Instructor,Admin")]
    [HttpPost("edit/{id:guid}")]
    public async Task<IActionResult> Edit(Guid id, UpdateLessonDto dto, CancellationToken ct)
    {
        if (id != dto.Id)
        {
            return BadRequest();
        }

        if (!ModelState.IsValid)
        {
            await PopulateCoursesDropdown(ct);
            return View(dto);
        }

        var result = await mediator.Send(new UpdateLesson.Command(dto), ct);
        if (result.IsSuccess)
        {
            return RedirectToAction(nameof(Index));
        }

        ModelState.AddModelError(string.Empty, result.Error);
        await PopulateCoursesDropdown(ct);
        return View(dto);
    }

    [HttpGet("details/{id:guid}")]
    public async Task<IActionResult> Details(Guid id, CancellationToken ct)
    {
        var result = await mediator.Send(new GetLessonById.Query(id), ct);
        if (!result.IsSuccess)
        {
            return NotFound();
        }

        return View(result.Value);
    }

    [Authorize(Roles = "Instructor,Admin")]
    [HttpGet("delete/{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        var result = await mediator.Send(new GetLessonById.Query(id), ct);
        if (!result.IsSuccess)
        {
            return NotFound();
        }

        return View(result.Value);
    }

    [Authorize(Roles = "Instructor,Admin")]
    [HttpPost("delete/{id:guid}"), ActionName("Delete")]
    public async Task<IActionResult> DeleteConfirmed(Guid id, CancellationToken ct)
    {
        var result = await mediator.Send(new DeleteLesson.Command(id), ct);
        if (result.IsSuccess)
        {
            return RedirectToAction(nameof(Index));
        }

        ModelState.AddModelError(string.Empty, result.Error);
        return RedirectToAction(nameof(Delete), new { id });
    }
}
