using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Sabatex.RadzenBlazor.Models;
using System.Security.Claims;

namespace Sabatex.RadzenBlazor;

// This is a client-side AuthenticationStateProvider that determines the user's authentication state by
// looking for data persisted in the page when it was rendered on the server. This authentication state will
// be fixed for the lifetime of the WebAssembly application. So, if the user needs to log in or out, a full
// page reload is required.
//
// This only provides a user name and email for display purposes. It does not actually include any tokens
// that authenticate to the server when making subsequent requests. That works separately using a
// cookie that will be included on HttpClient requests to the server.
public class PersistentAuthenticationStateProvider : AuthenticationStateProvider
{
    private static readonly Task<AuthenticationState> defaultUnauthenticatedTask =
        Task.FromResult(new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity())));

    private readonly Task<AuthenticationState> authenticationStateTask = defaultUnauthenticatedTask;

    IEnumerable<Claim> GetClaims(UserInfo userInfo)
    {
          yield return new Claim(ClaimTypes.NameIdentifier, userInfo.Id.ToString());
            yield return new Claim(ClaimTypes.Name, userInfo.Email);
            yield return new Claim(ClaimTypes.Email, userInfo.Email);
            foreach (var role in userInfo.Roles)
            {
                yield return new Claim(ClaimTypes.Role, role);
            }

    }


    public PersistentAuthenticationStateProvider(PersistentComponentState state)
    {
        if (!state.TryTakeFromJson<UserInfo>(nameof(UserInfo), out var userInfo) || userInfo is null)
        {
            return;
        }
        
        IEnumerable<Claim>  claims = GetClaims(userInfo);

        authenticationStateTask = Task.FromResult(
            new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity(claims,
                authenticationType: nameof(PersistentAuthenticationStateProvider)))));
    }

    public override Task<AuthenticationState> GetAuthenticationStateAsync() => authenticationStateTask;
}
