using System;
using System.Threading;
using System.Threading.Tasks;
using Courses.Application.Common.Models;
using Courses.Application.Review.Commands;
using Courses.Application.Review.DTOs;
using Courses.Application.Review.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Courses.Application.Course.Queries;

namespace Courses.Presentation.Controllers;

[Route("reviews")]
public class ReviewsController(IMediator mediator) : Controller
{
    private async Task PopulateDropdowns(CancellationToken ct)
    {
        var coursesResult = await mediator.Send(new GetCoursesLookup.Query(), ct);
        if (coursesResult.IsSuccess)
        {
            ViewBag.Courses = new SelectList(coursesResult.Value, "Id", "Name");
        }
        var studentsResult = await mediator.Send(new GetStudentsLookup.Query(), ct);
        if (studentsResult.IsSuccess)
        {
            ViewBag.Students = new SelectList(studentsResult.Value, "Id", "Name");
        }
    }

    [HttpGet]
    public async Task<IActionResult> Index([FromQuery] int page = 1, [FromQuery] int pageSize = 10, CancellationToken ct = default)
    {
        var result = await mediator.Send(new GetReviews.Query(page, pageSize), ct);
        if (!result.IsSuccess)
        {
            return BadRequest(result.Error);
        }
        return View(result.Value);
    }

    [HttpGet("create")]
    public async Task<IActionResult> Create(CancellationToken ct)
    {
        await PopulateDropdowns(ct);
        return View(new CreateReviewDto(Guid.Empty, Guid.Empty, 5, string.Empty, false));
    }

    [HttpPost("create")]
    public async Task<IActionResult> Create(CreateReviewDto dto, CancellationToken ct)
    {
        if (!ModelState.IsValid)
        {
            await PopulateDropdowns(ct);
            return View(dto);
        }

        var result = await mediator.Send(new CreateReview.Command(dto), ct);
        if (result.IsSuccess)
        {
            return RedirectToAction(nameof(Index));
        }

        ModelState.AddModelError(string.Empty, result.Error);
        await PopulateDropdowns(ct);
        return View(dto);
    }

    [HttpGet("edit/{id:guid}")]
    public async Task<IActionResult> Edit(Guid id, CancellationToken ct)
    {
        var result = await mediator.Send(new GetReviewById.Query(id), ct);
        if (!result.IsSuccess)
        {
            return NotFound();
        }

        await PopulateDropdowns(ct);
        var dto = new UpdateReviewDto(
            result.Value.Id,
            result.Value.Rating,
            result.Value.Comment,
            result.Value.IsApproved
        );

        return View(dto);
    }

    [HttpPost("edit/{id:guid}")]
    public async Task<IActionResult> Edit(Guid id, UpdateReviewDto dto, CancellationToken ct)
    {
        if (id != dto.Id)
        {
            return BadRequest();
        }

        if (!ModelState.IsValid)
        {
            await PopulateDropdowns(ct);
            return View(dto);
        }

        var result = await mediator.Send(new UpdateReview.Command(dto), ct);
        if (result.IsSuccess)
        {
            return RedirectToAction(nameof(Index));
        }

        ModelState.AddModelError(string.Empty, result.Error);
        await PopulateDropdowns(ct);
        return View(dto);
    }

    [HttpGet("details/{id:guid}")]
    public async Task<IActionResult> Details(Guid id, CancellationToken ct)
    {
        var result = await mediator.Send(new GetReviewById.Query(id), ct);
        if (!result.IsSuccess)
        {
            return NotFound();
        }

        return View(result.Value);
    }

    [HttpGet("delete/{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        var result = await mediator.Send(new GetReviewById.Query(id), ct);
        if (!result.IsSuccess)
        {
            return NotFound();
        }

        return View(result.Value);
    }

    [HttpPost("delete/{id:guid}"), ActionName("Delete")]
    public async Task<IActionResult> DeleteConfirmed(Guid id, CancellationToken ct)
    {
        var result = await mediator.Send(new DeleteReview.Command(id), ct);
        if (result.IsSuccess)
        {
            return RedirectToAction(nameof(Index));
        }

        ModelState.AddModelError(string.Empty, result.Error);
        return RedirectToAction(nameof(Delete), new { id });
    }
}
