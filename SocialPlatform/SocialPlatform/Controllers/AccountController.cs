using Microsoft.AspNetCore.Mvc;
using SocialPlatform.Models;
using SocialPlatform.Services;

namespace SocialPlatform.Controllers;

public class AccountController : Controller
{
    private readonly AccountService _accountService;
    private string? _sessionId;

    public AccountController(AccountService accountService)
    {
        _accountService = accountService;
    }

    public async Task<ViewResult> Index()
    {
        var sessionId = default(string);

        if (_sessionId is not null
            || Request.Cookies.TryGetValue(nameof(Data.Entities.User.SessionId), out sessionId))
        {
            var userId = await _accountService.TryGetUserIdBySessionId(_sessionId ?? sessionId!);
            
            if (userId != null)
            {
                return View(nameof(Index));
            }
        }

        return SignIn();
    }

    public ViewResult SignIn()
    {
        return View(nameof(SignIn));
    }

    [HttpPost]
    public async Task<RedirectResult> LogIn(LogInRequest request)
    {
        var sessionId = await _accountService.TryLogIn(request);

        if (sessionId is not null)
        {
            var options = new CookieOptions()
            {
                Expires = DateTime.UtcNow.AddDays(1),
                Domain = Request.Host.Host,
            };

            Response.Cookies.Append(
                nameof(Data.Entities.User.SessionId),
                sessionId,
                options);

            _sessionId = sessionId;
        }

        return Redirect("/Account/Index");
    }

    public ViewResult SignUp()
    {
        return View(nameof(SignUp));
    }

    [HttpPost]
    public async Task<RedirectResult> Register(RegisterRequest request)
    {
        if (await _accountService.TryRegister(request))
        {
            return await LogIn(new LogInRequest
            {
                Email = request.Email,
                Password = request.Password,
            });
        }
        else
        {
            return Redirect("/Account/SignUp");
        }
    }

    public ViewResult EditUser()
    {
        return View(nameof(EditUser));
    }

    [HttpPost]
    public async Task<RedirectResult> Update(RegisterRequest request)
    {
        if (Request.Cookies.TryGetValue(nameof(Data.Entities.User.SessionId), out var sessionId))
        {
            var userId = await _accountService.TryGetUserIdBySessionId(sessionId!);

            await _accountService.TryUpdate(
                request.Email,
                request.Password,
                request.Handle,
                request.DisplayName,
                userId!);
        }

        return Redirect("/Account/Index");
    }
}
