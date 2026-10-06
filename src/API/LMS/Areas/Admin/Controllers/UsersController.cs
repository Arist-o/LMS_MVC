using Identity.Application.Common.Models;
using Identity.Application.Users.Commands;
using Identity.Application.Users.DTOs;
using Identity.Application.Users.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LMS.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Admin")]
[Route("admin/users")]
public class UsersController(IMediator mediator) : Controller
{
    [HttpGet]
    public async Task<IActionResult> Index([FromQuery] GetUsers.Query query, CancellationToken ct)
    {
        var result = await mediator.Send(query, ct);
        if (!result.IsSuccess)
        {
            ModelState.AddModelError(string.Empty, result.Error);
            return View(new PagedList<UserDto>([], 0, query.Page == 0 ? 1 : query.Page, query.PageSize == 0 ? 10 : query.PageSize));
        }

        return View(result.Value);
    }

    [HttpPost("{id:guid}/toggle-block")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleBlock(Guid id, CancellationToken ct)
    {
        var result = await mediator.Send(new ToggleUserBlock.Command(id), ct);
        if (!result.IsSuccess)
        {
            TempData["ErrorMessage"] = result.Error;
        }
        else
        {
            TempData["SuccessMessage"] = result.Value ? "Користувача заблоковано" : "Користувача розблоковано";
        }
        
        return RedirectToAction(nameof(Index));
    }

    [HttpPost("{id:guid}/change-role")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ChangeRole(Guid id, [FromForm] string role, CancellationToken ct)
    {
        var result = await mediator.Send(new ChangeUserRole.Command(id, role), ct);
        if (!result.IsSuccess)
        {
            TempData["ErrorMessage"] = result.Error;
        }
        else
        {
            TempData["SuccessMessage"] = "Роль успішно змінено";
        }
        
        return RedirectToAction(nameof(Index));
    }
}
