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

    public IActionResult Help()
    {
        return View("Help");
    }

}
