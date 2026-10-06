using Identity.Application.Auth.Commands;
using Identity.Application.Auth.DTOs;
using Identity.Application.Auth.Queries;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Identity.Presentation.Controllers;

[Route("auth")]
public class AuthController(IMediator mediator) : Controller
{
    [HttpGet("login")]
    public IActionResult Login()
    {
        return View(new LoginDto(string.Empty, string.Empty));
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDto dto, CancellationToken ct)
    {
        if (!ModelState.IsValid)
            return View(dto);

        var query = new LoginUser.Query(dto);
        var result = await mediator.Send(query, ct);

        if (result.IsSuccess)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true, // Better for production
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.UtcNow.AddHours(2)
            };
            Response.Cookies.Append("jwt", result.Value.Token, cookieOptions);
            
            return Redirect("/");
        }

        ModelState.AddModelError(string.Empty, result.Error);
        return View(dto);
    }

    [HttpGet("register")]
    public IActionResult Register()
    {
        return View(new RegisterDto(string.Empty, string.Empty, "Student", string.Empty, string.Empty));
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterDto dto, CancellationToken ct)
    {
        if (!ModelState.IsValid)
            return View(dto);

        var command = new RegisterUser.Command(dto);
        var result = await mediator.Send(command, ct);

        if (result.IsSuccess)
        {
            return RedirectToAction(nameof(Login));
        }

        ModelState.AddModelError(string.Empty, result.Error);
        return View(dto);
    }

    [HttpPost("logout")]
    public IActionResult Logout()
    {
        Response.Cookies.Delete("jwt");
        return Redirect("/");
    }
}
