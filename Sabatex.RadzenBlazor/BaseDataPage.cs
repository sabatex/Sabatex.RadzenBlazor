using Microsoft.AspNetCore.Components;
using Radzen;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Xml.Linq;


namespace Sabatex.RadzenBlazor;

public class BaseDataPage:ComponentBase
{
    [Inject]
    protected HttpClient? httpClient { get; set; }
    [Inject]
    protected NavigationManager? navigationManager { get; set; }
    [Inject]
    protected NotificationService? NotificationService { get; set; }


    private Uri? baseUri;

    protected override void OnInitialized()
    {
        base.OnInitialized();
        baseUri = new Uri($"{navigationManager.BaseUri}odata/");
    }
   
}
