using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MVC_B2C_PasswordManager.Contexts.Models;
using MVC_B2C_PasswordManager.Interfaces;
using MVC_B2C_PasswordManager.Utils;

namespace MVC_B2C_PasswordManager.Controllers;

[Authorize]
public class PasswordManagerController : Controller
{
    private readonly ILogger<PasswordManagerController> logger;
    private readonly IPasswordManagerAccountRepository<PasswordmanagerAccount> passwordManagerAccountRepository;

    public PasswordManagerController(ILogger<PasswordManagerController> logger, IPasswordManagerAccountRepository<PasswordmanagerAccount> passwordManagerAccountRepository)
    {
        this.logger = logger;
        this.passwordManagerAccountRepository = passwordManagerAccountRepository;
    }

    public async Task<IActionResult> PasswordTable()
    {
        var userid = HttpContext.User.Claims.First(c=>c.Type == ClaimTypes.NameIdentifier).Value;
        var accounts = await passwordManagerAccountRepository.GetAccountsAsync(userid);

        ViewBag.userid = userid;

        return View(accounts);
    }

    [HttpPost]
    public async Task<IActionResult> UpdatePasswordAccount(PasswordmanagerAccount model)
    {
        var response = await passwordManagerAccountRepository.UpdateAsync(model);

        if (response is null)
        {
            return BadRequest();
        }

        return RedirectToAction(nameof(PasswordTable));
    }

    // [HttpPost]
    // [Route("create-password")]
    // public async Task<IActionResult> AddPasswordAccount(PasswordmanagerAccount model)
    // {
    //     var response = await passwordManagerAccountRepository.CreateAsync(model);

    //     if (response is null)
    //     {
    //         return BadRequest();
    //     }

    //     return Ok(response);
    // }

    // [HttpPost("[action]/single/{UserId}")]
    // [AllowAnonymous]
    // public async Task<IActionResult> Upload(IFormFile file, string UserId)
    // {
    //     var result = await passwordManagerAccountRepository.UploadCsvAsync(file, UserId);

    //     if (result is null)
    //     {
    //         return BadRequest("failed to upload csv");
    //     }

    //     return Ok("upload csv success!");
    // }

    [HttpPost]
    public async Task<IActionResult> DeletePasswordAccount(PasswordmanagerAccount model)
    {
        var response = await passwordManagerAccountRepository.DeleteAsync(model);

        if (response is null)
        {
            return BadRequest();
        }

        return RedirectToAction(nameof(PasswordTable));
    }
}
