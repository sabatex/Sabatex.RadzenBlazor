using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Sabatex.RadzenBlazor;

// This class provides an example of how JavaScript functionality can be wrapped
// in a .NET class for easy consumption. The associated JavaScript module is
// loaded on demand when first needed.
//
// This class can be registered as scoped DI service and then injected into Blazor
// components for use.

public class sabatexJsInterop : IAsyncDisposable
{
    private readonly Lazy<Task<IJSObjectReference>> moduleTask;

    public sabatexJsInterop(IJSRuntime jsRuntime)
    {
        moduleTask = new(() => jsRuntime.InvokeAsync<IJSObjectReference>(
            "import", "./_content/sabatex.RadzenBlazor.UIExt/exampleJsInterop.js").AsTask());
    }

    public async ValueTask<string> Prompt(string message)
    {
        var module = await moduleTask.Value;
        return await module.InvokeAsync<string>("showPrompt", message);
    }

    //public async ValueTask FocusElement(ElementReference elementReference)
    //{
    //    var module = await moduleTask.Value;
    //   await module.InvokeAsync<string>("focusElement", elementReference);
    //}
    public async ValueTask FocusElement(string elementReference)
    {
        var module = await moduleTask.Value;
        await module.InvokeAsync<string>("focusElement", elementReference);
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