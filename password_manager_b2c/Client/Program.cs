using Blazored.LocalStorage;
using Blazored.Modal;
using Blazored.SessionStorage;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using password_manager_b2c.Client;
using password_manager_b2c.Client.Auth;
using password_manager_b2c.Client.Interfaces;
using password_manager_b2c.Client.Services;
using password_manager_b2c.Shared;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddBlazoredSessionStorage();
builder.Services.AddBlazoredLocalStorage();

builder.Services.AddScoped<IAuthService, AuthService>();

builder.Services.AddScoped<IPasswordAccountApiService<PasswordmanagerAccount>, PasswordAccountApiService>();

builder.Services.AddBlazoredModal();

builder.Services
    .AddHttpClient(
        "password_manager_b2c.ServerAPI",
        client => client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress)
    )
    .AddHttpMessageHandler<BaseAddressAuthorizationMessageHandler>();

// Supply HttpClient instances that include access tokens when making requests
// to the server project
builder.Services.AddScoped(
    sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient("password_manager_b2c.ServerAPI")
);

builder.Services.AddMsalAuthentication(options =>
{
    builder.Configuration.Bind("AzureAdB2C", options.ProviderOptions.Authentication);
    options.ProviderOptions.DefaultAccessTokenScopes.Add(
        "https://testleoname.onmicrosoft.com/06a487c7-7860-466a-90a6-ee14c0148446/Ap.ReadWrite"
    );
    options.ProviderOptions.Cache.CacheLocation = "localStorage";
    options.ProviderOptions.LoginMode = "redirect";
});

await builder.Build().RunAsync();
