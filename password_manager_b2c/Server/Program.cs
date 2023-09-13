using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Web;
using password_manager_b2c.Server.Contexts;
using password_manager_b2c.Server.Repositories;
using password_manager_b2c.Shared;


var builder = WebApplication.CreateBuilder(args);

/*
dotnet ef dbcontext scaffold "Host=localhost;Database=mydatabase;Username=myuser;Password=mypassword" Npgsql.EntityFrameworkCore.PostgreSQL -o Models
*/

builder.Services.AddDbContext<EfDbContext>();

builder.Services.AddScoped<IPasswordManagerAccountRepository<PasswordmanagerAccount>, PasswordManagerAccountRepository>();
builder.Services.AddSingleton<DapperContext>();
builder.Services.AddSingleton<EncryptionContext>();
builder.Services.AddHttpContextAccessor();


// Add services to the container.
builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApi(builder.Configuration.GetSection("AzureAdB2C"));

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for
    // production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseBlazorFrameworkFiles();
app.UseStaticFiles();

app.UseRouting();

// app.UseSession();

app.UseAuthentication();
app.UseAuthorization();

// test authentication out
// app.Use(
//     async (ctx, next) =>
//     {
//         if (!ctx.User.Identity?.IsAuthenticated ?? false)
//         {
//             ctx.Response.StatusCode = 401;
//             await ctx.Response.WriteAsync("Not authenticated");
//         }
//         else
//             await next();
//     }
// );

app.UseAuthorization();

app.MapRazorPages();
app.MapControllers();
app.MapFallbackToFile("index.html");

app.Run();
