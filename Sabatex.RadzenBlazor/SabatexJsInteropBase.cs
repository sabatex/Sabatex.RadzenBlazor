using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Sabatex.RadzenBlazor;

// This class can be registered as scoped DI service and then injected into Blazor
// components for use.

public class SabatexJsInteropBase : IAsyncDisposable
{
    protected readonly Lazy<Task<IJSObjectReference>> moduleTask;
    
    public SabatexJsInteropBase(IJSRuntime jsRuntime)
    {
        moduleTask = new(() => jsRuntime.InvokeAsync<IJSObjectReference>("import",
            $"./_content/Sabatex.RadzenBlazor/Sabatex.RadzenBlazor.js?v={(typeof(PWAPushService).Assembly.GetName().Version)}").AsTask());
    }

 
    public async ValueTask DisposeAsync()
    {
        if (moduleTask.IsValueCreated)
        {
            var module = await moduleTask.Value;
            await module.DisposeAsync();
        }
    }
}