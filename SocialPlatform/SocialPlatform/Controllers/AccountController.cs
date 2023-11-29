using Microsoft.AspNetCore.Mvc;

namespace SocialPlatform.Controllers;

public class AccountController : Controller
{
    public ViewResult Index()
    {
        return View();
    }
}
