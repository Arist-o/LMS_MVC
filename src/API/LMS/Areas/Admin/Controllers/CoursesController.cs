using Courses.Application.Common.Models;
using Courses.Application.Course.Commands;
using Courses.Application.Course.DTOs;
using Courses.Application.Course.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace LMS.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Admin")]
[Route("admin/courses")]
public class CoursesController(IMediator mediator) : Controller
{
    [HttpGet]
    public async Task<IActionResult> Index([FromQuery] GetCourses.Query query, CancellationToken ct)
    {
        var result = await mediator.Send(query, ct);
        if (!result.IsSuccess)
        {
            ModelState.AddModelError(string.Empty, result.Error);
            return View(new PagedList<CourseResponseDto>([], 0, query.Page == 0 ? 1 : query.Page, query.PageSize == 0 ? 10 : query.PageSize));
        }

        return View(result.Value);
    }

    private async Task PopulateInstructorsDropdown(CancellationToken ct)
    {
        var result = await mediator.Send(new GetInstructorsLookup.Query(), ct);
        if (result.IsSuccess)
        {
            ViewBag.Instructors = new SelectList(result.Value, "Id", "Name");
        }
    }

    [HttpGet("create")]
    public async Task<IActionResult> Create(CancellationToken ct)
    {
        await PopulateInstructorsDropdown(ct);
        return View(new CreateCourseDto(Guid.Empty, string.Empty, string.Empty, false));
    }

    [HttpPost("create")]
    public async Task<IActionResult> Create(CreateCourseDto dto, CancellationToken ct)
    {
        if (!ModelState.IsValid)
        {
            await PopulateInstructorsDropdown(ct);
            return View(dto);
        }

        var command = new CreateCourse.Command(dto);
        var result = await mediator.Send(command, ct);

        if (result.IsSuccess)
            return RedirectToAction(nameof(Index));

        ModelState.AddModelError(string.Empty, result.Error);
        await PopulateInstructorsDropdown(ct);
        return View(dto);
    }

    [HttpGet("{id:guid}/edit")]
    public async Task<IActionResult> Edit(Guid id, CancellationToken ct)
    {
        var result = await mediator.Send(new GetCourseById.Query(id), ct);
        if (!result.IsSuccess) return NotFound();

        await PopulateInstructorsDropdown(ct);
        var dto = new UpdateCourseDto(result.Value.Id, result.Value.Title, result.Value.Description, result.Value.IsPublished);
        return View(dto);
    }

    [HttpPost("{id:guid}/edit")]
    public async Task<IActionResult> Edit(Guid id, UpdateCourseDto dto, CancellationToken ct)
    {
        if (id != dto.Id) return BadRequest();

        if (!ModelState.IsValid)
        {
            await PopulateInstructorsDropdown(ct);
            return View(dto);
        }

        var command = new UpdateCourse.Command(dto);
        var result = await mediator.Send(command, ct);

        if (result.IsSuccess)
            return RedirectToAction(nameof(Index));

        ModelState.AddModelError(string.Empty, result.Error);
        await PopulateInstructorsDropdown(ct);
        return View(dto);
    }

    [HttpGet("{id:guid}/delete")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        var result = await mediator.Send(new GetCourseById.Query(id), ct);
        if (!result.IsSuccess) return NotFound();
        return View(result.Value);
    }

    [HttpPost("{id:guid}/delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(Guid id, CancellationToken ct)
    {
        var command = new DeleteCourse.Command(id);
        var result = await mediator.Send(command, ct);

        if (result.IsSuccess)
            return RedirectToAction(nameof(Index));

        ModelState.AddModelError(string.Empty, result.Error);
        var course = await mediator.Send(new GetCourseById.Query(id), ct);
        return View("Delete", course.IsSuccess ? course.Value : null);
    }
}
