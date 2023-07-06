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

    [Inject]
    protected Blazored.LocalStorage.ISyncLocalStorageService? LocalStorageService { get; set; }

    protected string PageName = "Default";

    private Uri? baseUri;

    private int columnsPerPage;
    protected int ColumnsPerPage
    {
        get=> columnsPerPage;
        set
        {
            columnsPerPage = value;
            LocalStorageService?.SetItem(PageName + nameof(ColumnsPerPage),value);
        }
    }


    protected override void OnInitialized()
    {
        base.OnInitialized();
        columnsPerPage = LocalStorageService?.GetItem<int>(PageName + nameof(ColumnsPerPage)) ?? 10;
        if (navigationManager != null)
        {
            baseUri = new Uri($"{navigationManager.BaseUri}odata/");
        }
        else
        {
            throw new Exception("Sabatex.RadzenBlazor->BaseDataPage->OnInitialized() navigationManager is NULL");
        }
    }
   
}
