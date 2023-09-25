using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MVC_B2C_PasswordManager.Contexts.Models;

namespace MVC_B2C_PasswordManager.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> logger;

    public HomeController(ILogger<HomeController> logger)
    {
        this.logger = logger;
    }

    public IActionResult Index()
    {
        return View();
    }

    public IActionResult ChangeTheme()
    {
        return View();
    }

    [HttpPost]
    public IActionResult SetTheme(string data)
    {
        var cookies = new CookieOptions();
        cookies.Expires = DateTime.Now.AddDays(1);

        Response.Cookies.Append("theme", data, cookies);

        return Ok();
    }

    public IActionResult Help()
    {
        return View("Help");
    }

}
