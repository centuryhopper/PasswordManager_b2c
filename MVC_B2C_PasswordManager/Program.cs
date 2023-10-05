using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.UI;
using MVC_B2C_PasswordManager.Server.Contexts;
using MVC_B2C_PasswordManager.Contexts.Models;
using MVC_B2C_PasswordManager.Server.Repositories;
using MVC_B2C_PasswordManager.Interfaces;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<EfDbContext>();
builder.Services.AddSingleton<EncryptionContext>();
builder.Services.AddScoped<IPasswordManagerAccountRepository<PasswordmanagerAccount>, PasswordManagerAccountRepository>();

// Add services to the container.
builder.Services.AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApp(builder.Configuration.GetSection("AzureAdB2C"));
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages()
    .AddMicrosoftIdentityUI();

builder.Services.AddAuthorization(options => {
    options.AddPolicy("admin-only", p => {
        p.RequireClaim("groups", "ea55a0f8-0b89-4776-872a-36ed86dfe46b");
    });
});

builder.Services.AddHttpContextAccessor();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();
