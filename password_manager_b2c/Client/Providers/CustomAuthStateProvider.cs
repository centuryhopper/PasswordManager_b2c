using System.Security.Claims;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using password_manager_b2c.Shared;

namespace password_manager_b2c.Client.Providers;

public class CustomAuthStateProvider : AuthenticationStateProvider
{
    private readonly HttpClient httpClient;
    private readonly ILocalStorageService localStorageService;
    private ClaimsPrincipal claimsPrincipal;

    public CustomAuthStateProvider(IHttpClientFactory httpClientFactory, ILocalStorageService localStorageService)
    {
        claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity());
        this.httpClient = httpClientFactory.CreateClient("API");
        this.localStorageService = localStorageService;
    }

    public override Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        return Task.FromResult(new AuthenticationState(claimsPrincipal));
    }

    public void MarkUserAsAuthenticated(AuthStatus authStatus)
    {
        var identity = new ClaimsIdentity(new[] {

            new Claim(ClaimTypes.Email, authStatus.Email),

            new Claim(ClaimTypes.Name, $"{authStatus.Name}"),

            new Claim(ClaimTypes.Role, authStatus.Role),

            new Claim(ClaimTypes.NameIdentifier, authStatus.Id),

        }, "AuthCookie");

        claimsPrincipal = new ClaimsPrincipal(identity);
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());

    }

    public void MarkUserAsLoggedOut()
    {

        claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity());
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }
}
