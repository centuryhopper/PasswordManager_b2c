using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using password_manager_b2c.Client.Providers;
using Newtonsoft.Json;
using password_manager_b2c.Client.Interfaces;
using password_manager_b2c.Shared;
using System.Net.Http.Json;

namespace password_manager_b2c.Client.Services;

public class PasswordAccountApiService : IPasswordAccountApiService<PasswordmanagerAccount>
{
    private readonly HttpClient client;

    public PasswordAccountApiService(IHttpClientFactory httpClientFactory)
    {
        this.client = httpClientFactory.CreateClient("password_manager_b2c.ServerAPI");
    }

    public async Task<PasswordmanagerAccount> CreateAsync(PasswordmanagerAccount model)
    {
        System.Console.WriteLine($"creating password: {model}");
        var response = await client.PostAsJsonAsync("/api/PasswordManager/create-password", model);
        if (response.IsSuccessStatusCode)
        {
            Console.WriteLine("success creating password");
            var result = await response.Content.ReadFromJsonAsync<PasswordmanagerAccount?>();
            return result;
        }

        return new PasswordmanagerAccount{};
    }

    public async Task<PasswordmanagerAccount> DeleteAsync(PasswordmanagerAccount model)
    {
        var response = await client.PostAsJsonAsync("/api/PasswordManager/delete-password", model);
        if (response.IsSuccessStatusCode)
        {
            var result = await response.Content.ReadFromJsonAsync<PasswordmanagerAccount?>();
            return result;
        }
        else
        {
            return null;
        }
    }

    public async Task<IEnumerable<PasswordmanagerAccount>> GetPasswordAccounts(string UserId)
    {
        var response = await client.GetAsync($"/api/PasswordManager/get-passwordaccounts/{UserId}");

        if (response.IsSuccessStatusCode)
        {
            var result = await response.Content.ReadAsStringAsync();
            if (string.IsNullOrEmpty(result))
            {
                return Enumerable.Empty<PasswordmanagerAccount>();
            }

            var ans = JsonConvert.DeserializeObject<IEnumerable<PasswordmanagerAccount>>(result)!;

            return ans;
        }
        else
        {
            return Enumerable.Empty<PasswordmanagerAccount>();
        }
    }

    public async Task<PasswordmanagerAccount?> UpdateAsync(PasswordmanagerAccount? model)
    {
        var response = await client.PutAsJsonAsync("/api/PasswordManager/update-password", model);
        if (response.IsSuccessStatusCode)
        {
            var result = await response.Content.ReadFromJsonAsync<PasswordmanagerAccount?>();
            return result;
        }
        else
        {
            return null;
        }
    }
}