using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MVC_B2C_PasswordManager.Contexts.Models;
using MVC_B2C_PasswordManager.Interfaces;
using MVC_B2C_PasswordManager.Models;
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

    public async Task<IActionResult> PasswordTable(int pg=2)
    {
        var userid = HttpContext.User.Claims.First(c=>c.Type == ClaimTypes.NameIdentifier).Value;

        var accounts = await passwordManagerAccountRepository.GetAccountsAsync(userid);

        const int PAGE_SIZE = 3;
        if (pg < 1)
        {
            pg = 1;
        }

        var pager = new Pager(accounts.Count(), pg, PAGE_SIZE);
        int recSkip = (pg-1)*PAGE_SIZE;

        var dto = await passwordManagerAccountRepository.GetAccountsAsync(userid, recSkip, pager.PageSize);

        ViewBag.Pager = pager;
        ViewBag.userid = userid;

        return View(dto);
    }

    [HttpPost]
    public async Task<IActionResult> UpdatePasswordAccount(PasswordmanagerAccount model)
    {
        var userid = HttpContext.User.Claims.First(c=>c.Type == ClaimTypes.NameIdentifier).Value;

        var response = await passwordManagerAccountRepository.UpdateAsync(new PasswordmanagerAccount {
            Id = model.Id,
            Userid = userid,
            Title = model.Title,
            Username = model.Username,
            Password = model.Password,
            CreatedAt = model.Title,
        });

        if (response is null)
        {
            TempData[TempDataKeys.ALERT_ERROR] = "Failed to update this password account";
        }

        return RedirectToAction(nameof(PasswordTable));
    }

    [HttpPost]
    public async Task<IActionResult> AddRows(int numRows)
    {
        // logger.LogWarning(numRows+"");
        var userid = HttpContext.User.Claims.First(c=>c.Type == ClaimTypes.NameIdentifier).Value;

        for (int i = 0; i < numRows; i++)
        {
            var response = await passwordManagerAccountRepository.CreateAsync(new PasswordmanagerAccount {
                Userid = userid,
                Title = "enter title",
                Username = "enter username",
                Password = "enter password",
            });
        }

        // logger.LogWarning(response+"");

        return RedirectToAction(nameof(PasswordTable));
    }

    [HttpPost]
    public async Task<IActionResult> DeletePasswordAccount(PasswordmanagerAccount model)
    {
        var response = await passwordManagerAccountRepository.DeleteAsync(model);

        if (response is null)
        {
            TempData[TempDataKeys.ALERT_ERROR] = "Failed to delete this password account";
        }

        return RedirectToAction(nameof(PasswordTable));
    }

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
}
