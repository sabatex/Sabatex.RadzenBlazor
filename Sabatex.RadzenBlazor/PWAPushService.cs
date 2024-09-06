using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Sabatex.RadzenBlazor.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Threading.Tasks;


namespace Sabatex.RadzenBlazor;

public class PWAPushService : IPWAPush,IAsyncDisposable
{
    private readonly Lazy<Task<IJSObjectReference>> moduleTask;
    private readonly HttpClient _httpClient;
    record Cert(string cert);
    public PWAPushService(IJSRuntime jsRuntime, HttpClient httpClient)
    {
        moduleTask = new(() => jsRuntime.InvokeAsync<IJSObjectReference>(
            "import", $"./_content/Sabatex.RadzenBlazor/Sabatex.RadzenBlazor.js?v={(typeof(PWAPushService).Assembly.GetName().Version)}").AsTask());
        _httpClient = httpClient;
    }

    public async ValueTask DisposeAsync()
    {
        if (moduleTask.IsValueCreated)
        {
            var module = await moduleTask.Value;
            await module.DisposeAsync();
        }

    }

    public async Task<PWAPushHandler?> GetSubscriptionAsync()
    {
        var module = await moduleTask.Value;
        return await module.InvokeAsync<PWAPushHandler>("sabatexPWAPush.getSubscription");
    }
    public async Task<PWAPushHandler?> SubscribeAsync()
    {
        var module = await moduleTask.Value;
        var cert = await _httpClient.GetFromJsonAsync<Cert>("/api/push/public_key");
        var subscrtion = await module.InvokeAsync<PWAPushHandler>("sabatexPWAPush.subscribe",cert.cert);
        await _httpClient.PutAsJsonAsync("api/push/subscribe", subscrtion);
        return subscrtion;
    }
    
    /// <summary>
    /// update subscription
    /// </summary>
    /// <returns></returns>
    public async Task UpdateSubscriptionAsync()
    {
        var subscrtion = await GetSubscriptionAsync();
        if (subscrtion == null) 
        {
            subscrtion = await SubscribeAsync();
        }
        var response = await _httpClient.PutAsJsonAsync("api/push/subscribe", subscrtion);
    }

    /// <summary>
    /// Clear subscription on all devices
    /// </summary>
    /// <returns></returns>
    public async Task ClearSubscriptionAsync()
    {
        var module = await moduleTask.Value;
        await module.InvokeAsync<object>("sabatexPWAPush.unsubscribe");
        await _httpClient.PostAsync("api/push/clearSubscribe",null);
    }

    public async Task UnSubscribeAsync()
    {
        var module = await moduleTask.Value;
        var subscrtion = await GetSubscriptionAsync();
        await module.InvokeAsync<object>("sabatexPWAPush.unsubscribe");
        await _httpClient.PutAsJsonAsync("api/push/unsubscribe", subscrtion);

    }
}
