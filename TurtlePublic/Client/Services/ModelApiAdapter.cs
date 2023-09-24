using Generated;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Json;
using System.Security.Claims;
using TurtlePublic.Extensions;

namespace TurtlePublic.Services;

public class ModelApiAdapter : AuthenticationStateProvider, IModelApiAdapter
{
    private readonly HttpClient http;
    private readonly IJSRuntime js;
    public ModelApiAdapter(HttpClient http, IJSRuntime js)
    {
        this.http = http;
        this.js = js;
    }

    internal record ProblemDetails(
        string type,
        string title,
        int status,
        string detail,
        string instance);

    private async Task PropagateErrorAsync(HttpContent response)
    {
        var content = await response.ReadAsStringAsync();
        var problem = content.FromJson<ProblemDetails>();

        if (string.IsNullOrEmpty(problem.detail))
        {
            throw new Exception(problem.title);
        } else
        {
            throw new Exception(problem.detail);
        }
    }

    public async Task ExecuteAsync(string path, object args)
    {
        var result = await http.PostAsJsonAsync(path, args);
        if (!result.IsSuccessStatusCode)
        {
            await PropagateErrorAsync(result.Content);
        }
    }

    public async Task<T> ExecuteAsync<T>(string path, object args)
    {
        var result = await http.PostAsJsonAsync(path, args);
        if (!result.IsSuccessStatusCode)
        {
            await PropagateErrorAsync(result.Content);
        }
        var content = await result.Content.ReadAsStringAsync();
        return content.FromJson<T>();
    }

    public async Task StoreSessionTokenAsync(Account.WithSession session)
    {
        await js.InvokeVoidAsync("localStorage.setItem", "TurtleControl_Session", session.SessionToken);
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }

    public async Task<string> RetrieveSessionTokenAsync()
    {
        return await js.InvokeAsync<string>("localStorage.getItem", "TurtleControl_Session");
    }

    public async Task DestroySessionTokenAsync()
    {
        await js.InvokeVoidAsync("localStorage.removeItem", "TurtleControl_Session");
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var token = await RetrieveSessionTokenAsync();
        if (token is null)
        {
            goto unauthorized;
        }

        try
        {
            var handler = new JwtSecurityTokenHandler();
            var jwt = handler.ReadJwtToken(token);

            if (DateTime.UtcNow < jwt.ValidTo)
            {
                var identity = new ClaimsIdentity(jwt.Claims, "LoggedIn");
                var principal = new ClaimsPrincipal(identity);
                return new AuthenticationState(principal);
            } else
            {
                //TODO Refresh
                goto unauthorized;
            }
        } catch (Exception)
        {
            goto unauthorized;
        }

    unauthorized:
        return new(new());
    }
}