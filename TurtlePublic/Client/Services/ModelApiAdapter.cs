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
    private readonly IServiceProvider sp;
    public ModelApiAdapter(HttpClient http, IJSRuntime js, IServiceProvider sp)
    {
        this.http = http;
        this.js = js;
        this.sp = sp;
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

    // Set to avoid checking token expiry after discovering tokens are
    // soon to expire (allows refresh API call w/o infinite loop)
    private bool IgnoreTokens = false;

    public async Task ExecuteAsync(string path, object args)
    {
        if (!IgnoreTokens)
        {
            await GetAuthenticationStateAsync();
        }

        var result = await http.PostAsJsonAsync(path, args);
        if (!result.IsSuccessStatusCode)
        {
            await PropagateErrorAsync(result.Content);
        }
    }

    public async Task<T> ExecuteAsync<T>(string path, object args)
    {
        if (!IgnoreTokens)
        {
            await GetAuthenticationStateAsync();
        }

        var result = await http.PostAsJsonAsync(path, args);
        if (!result.IsSuccessStatusCode)
        {
            await PropagateErrorAsync(result.Content);
        }
        var content = await result.Content.ReadAsStringAsync();
        return content.FromJson<T>();
    }

    public async Task StoreSessionTokenAsync(Account.WithSession session, bool notify = true)
    {
        await js.InvokeVoidAsync("localStorage.setItem", "TurtleControl_Session", session.SessionToken);
        if (notify)
        {
            NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
        }
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

            if (DateTime.UtcNow.AddMinutes(30) > jwt.ValidTo)
            {
                IgnoreTokens = true;
                try
                {
                    var accounts = sp.GetRequiredService<Account.IService>();
                    // Client does not know the refresh token; it is inserted at the API
                    var refreshed = await accounts.Refresh(token, "");
                    await StoreSessionTokenAsync(refreshed, false);
                    jwt = handler.ReadJwtToken(refreshed.SessionToken);
                } finally
                {
                    IgnoreTokens = false;
                }
                
            }

            var identity = new ClaimsIdentity(jwt.Claims, "LoggedIn");
            var principal = new ClaimsPrincipal(identity);
            return new AuthenticationState(principal);
        } catch (Exception)
        {
            goto unauthorized;
        }

    unauthorized:
        return new(new());
    }
}