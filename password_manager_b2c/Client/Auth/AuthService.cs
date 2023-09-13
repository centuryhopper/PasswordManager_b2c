using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using System.Net.Http.Json;
using System.Text.Json;
using System.Security.Claims;
using password_manager_b2c.Shared;

namespace password_manager_b2c.Client.Auth;

public class AuthService : IAuthService
{
    private readonly HttpClient _httpClient;
    private readonly AuthenticationStateProvider _authenticationStateProvider;
    private readonly ILocalStorageService _localStorage;

    public AuthService(IHttpClientFactory httpClientFactory,
                        AuthenticationStateProvider authenticationStateProvider,
                        ILocalStorageService localStorage)
    {
        _httpClient = httpClientFactory.CreateClient("password_manager_b2c.ServerAPI");
        _authenticationStateProvider = authenticationStateProvider;
        _localStorage = localStorage;
    }

    public async void GetClaims()
    {
        var authState = await _authenticationStateProvider.GetAuthenticationStateAsync();
        var user = authState.User;
        var userId = user.Claims.FirstOrDefault(c => c.Type == "sub");

        Console.WriteLine(userId);
    }

    public async void GetJwtFromAAD()
    {
        Console.WriteLine("getting jwt");

        if (await _localStorage.ContainKeyAsync("jwt") != true)
        {
            var authState = await _authenticationStateProvider.GetAuthenticationStateAsync();
            var user = authState.User;

            var userId = user.Claims.FirstOrDefault(c => c.Type == "sub");

            Console.WriteLine(userId);

            // var userClaims = user.Claims;

            // foreach (var claim in userClaims)
            // {
            //     Console.WriteLine($"{claim.Type} : {claim.Value}");
            // }

            //Console.WriteLine("pulling tokens now!");
            var tokens = await _localStorage.KeysAsync();

            // Console.WriteLine("generating list of keys ");
            var tokenList = tokens.ToList();
            string accessTokenKey = "";

            foreach (var token in tokenList)
            {
                if (token.Contains("access") == true)
                {
                    // Console.WriteLine("Found the Access token!");
                    accessTokenKey = token;
                    break;
                }
            }

            Console.WriteLine(accessTokenKey);

            if (!string.IsNullOrEmpty(accessTokenKey))
            {
                var accessTokenString = await _localStorage.GetItemAsync<string>(accessTokenKey);
                //Console.WriteLine("ID Token: " + accessTokenString);

                AccessToken accessObject = JsonSerializer.Deserialize<AccessToken>(accessTokenString);
                //Console.WriteLine("Publishing untold secrets!");
                //Console.WriteLine(accessObject.secret);
                string jwt = accessObject.secret;
                await _localStorage.SetItemAsync("jwt", jwt);
            }
        }
        else
        {
            Console.WriteLine("Jwt exists already!");
        }


    }

    public async void GetClaimsFromAADJwt()
    {
        var authState = await _authenticationStateProvider.GetAuthenticationStateAsync();
        var user = authState.User;
        if (user.HasClaim(c => c.Type == ClaimTypes.Role) != true)
        {
            var jwt = await _localStorage.GetItemAsync<string>("jwt");

            if (jwt != null)
            {
                var payload = jwt.Split('.')[1];

                //Console.WriteLine(payload); // check for emission
                switch (payload.Length % 4)
                {
                    case 2: payload += "=="; break;
                    case 3: payload += "="; break;
                }
                //Console.WriteLine("converting from base64");
                var convertedPayload = Convert.FromBase64String(payload);
                //Console.WriteLine("to dictionary");
                var keyValuePairs = JsonSerializer.Deserialize<Dictionary<string, object>>(convertedPayload);

                keyValuePairs.TryGetValue("groups", out object groups);
                if (groups != null)
                {
                    if (groups.ToString().Trim().StartsWith("["))
                    {
                        var parsedGroups = JsonSerializer.Deserialize<string[]>(groups.ToString());

                        for (var i = 0; i < parsedGroups.Length; i++)
                        {
                            //Console.WriteLine(parsedGroups[i]);
                            if (parsedGroups[i] == "27c791e7-36f1-49e2-84f4-59456343f097")
                            {
                                //Add claim if they are
                                var claims = new List<Claim>
                            {
                                new Claim(ClaimTypes.Role, "FWRI_MIPS_SuperUser")
                            };
                                var appIdentity = new ClaimsIdentity(claims);
                                user.AddIdentity(appIdentity);
                            }
                            else if (parsedGroups[i] == "f9b28dfd-0b36-43e7-9a43-216bb6721ab2")
                            {

                                var claims = new List<Claim>
                            {
                                new Claim(ClaimTypes.Role, "FWRI_MIPS_ReadOnly")
                            };
                                var appIdentity = new ClaimsIdentity(claims);
                                user.AddIdentity(appIdentity);
                            }
                            else if (parsedGroups[i] == "d398b35e-6ea5-46ec-9567-27107cd862f7")
                            {
                                var claims = new List<Claim>
                            {
                                new Claim(ClaimTypes.Role, "FWRI_MIPS_Editor")
                            };
                                var appIdentity = new ClaimsIdentity(claims);
                                user.AddIdentity(appIdentity);

                            }
                        }



                    }
                }
                if (user.HasClaim(c => c.Type == ClaimTypes.Role) != true)
                {
                    GetClaims();
                    Console.WriteLine("claims can be retrieved");
                }

            }

        }

        else
        {
            //Console.Write("Role Claim exists already!");
        }



    }

    public void EstablishClaimsFromAAD()
    {
        GetJwtFromAAD();
        // GetClaimsFromAADJwt();
    }


}
