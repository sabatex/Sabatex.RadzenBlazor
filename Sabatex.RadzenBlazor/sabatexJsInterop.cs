using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Sabatex.RadzenBlazor;

// This class provides an example of how JavaScript functionality can be wrapped
// in a .NET class for easy consumption. The associated JavaScript module is
// loaded on demand when first needed.
//
// This class can be registered as scoped DI service and then injected into Blazor
// components for use.

public class SabatexJsInterop : IAsyncDisposable
{
    private readonly Lazy<Task<IJSObjectReference>> moduleTask;

    public SabatexJsInterop(IJSRuntime jsRuntime)
    {
        moduleTask = new(() => jsRuntime.InvokeAsync<IJSObjectReference>(
            "import", "./_content/Sabatex.RadzenBlazor/Sabatex.js").AsTask());
    }

    public async ValueTask<string> Prompt(string message)
    {
        var module = await moduleTask.Value;
        return await module.InvokeAsync<string>("showPrompt", message);
    }
    public async ValueTask FocusElement(string elementReference)
    {
        var module = await moduleTask.Value;
        await module.InvokeAsync<string>("focusElement", elementReference);
    }

    public async ValueTask DownloadStringAsFile(string fileName,string text){
        var module = await moduleTask.Value;
        await module.InvokeAsync<object>("sabatex.downloadStrigAsFile", fileName,text);
    }

    public async ValueTask<double> GetElementClientHeight(ElementReference element) {
        var module = await moduleTask.Value;
        return await module.InvokeAsync<double>("getElementClientHeight",element);  
    }
   public async ValueTask<double> GetElementOffSetHeight(ElementReference element) {
        var module = await moduleTask.Value;
        return await module.InvokeAsync<double>("getElementOffSetHeight",element);  
    }
  public async ValueTask<double> GetAvailHeight(ElementReference element) {
        var module = await moduleTask.Value;
        return await module.InvokeAsync<double>("getAvailHeight",element);  
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