using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Sabatex.RadzenBlazor
{
    public static class SabatexComponentExtension
    {
        public static async Task<string> GetUserIdAsync(this AuthenticationStateProvider stateProvider)
        {
            return (await stateProvider.GetAuthenticationStateAsync())?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty;
        }
        public static async Task<string> GetUserNameAsync(this AuthenticationStateProvider stateProvider)
        {
            return (await stateProvider.GetAuthenticationStateAsync())?.User.Identity?.Name ?? string.Empty;
        }

        public static async Task<bool> IsInRoreAsync(this AuthenticationStateProvider stateProvider,string role)
        {
            return (await stateProvider.GetAuthenticationStateAsync())?.User.IsInRole(role) ?? false;
        }
        public static async Task<bool> IsAuthenticatedAsync(this AuthenticationStateProvider stateProvider)
        {
            return (await stateProvider.GetAuthenticationStateAsync())?.User.Identity?.IsAuthenticated ?? false;
        }

    }
}
