using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using Radzen;
using Radzen.Blazor;
using Sabatex.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sabatex.RadzenBlazor;
/// <summary>
/// Base page with support grid
/// </summary>
/// <typeparam name="TItem"></typeparam>
/// <typeparam name="TKey"></typeparam>
public abstract class SabatexRadzenBlazorBaseGridPage<TItem, TKey> : SabatexRadzenBlazorBaseDataPage<TKey>,ISabatexRadzenGridPage  where TItem : class, IEntityBase<TKey>
{
    [Inject] 
    protected DialogService DialogService { get; set; } = default!;

    protected bool IsGridDataLoading = false;
    public bool IsGridRO {get;set;} = false;
    protected RadzenDataGrid<TItem> grid = default!;
    protected ODataServiceResult<TItem> dataCollection = new ODataServiceResult<TItem>();
    protected IList<TItem>? SelectedItems;
    protected virtual string? ExpandGrid => null;
    protected virtual ForeginKey? ForeginKey => null;

    private int columnsPerPage;
    protected virtual string EditPageUri 
    {
        get
        {
            var url = NavigationManager.ToBaseRelativePath(NavigationManager.Uri).ToLower();
            var index = url.LastIndexOf("/");
            string baseRoute = string.Empty;
            if (index != -1)
                baseRoute = url.Substring(0, index+1);
            //return $"{baseRoute}{typeof(TItem).Name}-edit";
            return $"{baseRoute}{EditPageName}";
        }
    }

    public virtual string EditPageName{get;set;}="edit"; 

    private void NavigateToEditPage(string? id)
    {
        string? idRoute = string.Empty;
        if (id != null)
            idRoute = $"/{id}";


        var queryParams = new Dictionary<string, object?>()
        {
            ["returnUrl"] = NavigationManager.ToBaseRelativePath(NavigationManager.Uri)
        };
        if (ForeginKey != null)
            queryParams.Add(ForeginKey.Name,ForeginKey.Id);


        var uri = NavigationManager.GetUriWithQueryParameters($"{EditPageUri}{idRoute}", queryParams);
 
        NavigationManager.NavigateTo(uri);
    }
    public virtual async Task AddButtonClick(MouseEventArgs args)
    {
        NavigateToEditPage(null);
        await Task.Yield();
    }
    protected virtual async Task RowDoubleClick(DataGridRowMouseEventArgs<TItem> args)
    {
        if (args.Data is not null)
            NavigateToEditPage(args.Data?.Id?.ToString());
        else
            throw new NullReferenceException("The args.Data is null");
 
        await Task.Yield();
    }
    protected virtual async Task EditButtonClick(TItem data)
    {
        if (data is not null)
            NavigateToEditPage(data.Id?.ToString());
        await Task.Yield();
    }
    protected virtual async Task DeleteButtonClick(TItem data)
    {
        try
        {
            if (DialogService == null)
            {
                throw new Exception("The DialogService=null sabatex.RadzenBlazor->InlineEditGridPage->GridDeleteButtonClick()");
            }
            if (await DialogService.Confirm("Ви впевнені?", "Видалення запису", new ConfirmOptions() { OkButtonText = "Так", CancelButtonText = "Ні" }) == true)
            {
                try
                {
                    await DataAdapter.DeleteAsync<TItem>(data.Id);
                    await grid.Reload();
                }
                catch (Exception e)
                {
                    NotificationService?.Notify(new NotificationMessage() { Severity = NotificationSeverity.Error, Summary = $"Error", Detail = $"Unable to delete TgClient with Error: {e.Message}" });
                }
            }
        }
        catch (System.Exception e)
        {
            NotificationService?.Notify(new NotificationMessage()
            {
                Severity = NotificationSeverity.Error,
                Summary = $"Помилка",
                Detail = $"Не можливо видалити Error:{e.Message}"
            });

        }
    }


    protected async Task GridLoadData(LoadDataArgs args)
    {
        IsGridDataLoading = true;
        IEnumerable<FieldDescriptor>? filterFields = null;
        try
        {


            if (dataCollection == null)
                throw new ArgumentNullException(nameof(dataCollection));
            string filter = string.Empty;
            if (filterFields != null)
            {
                // search in combobox
            }
            else
                filter = args.Filter;

            if (filterFields != null && !string.IsNullOrWhiteSpace(args.Filter))
            {
                ODataSearchFilterBuilder filterBuilder = new ODataSearchFilterBuilder();
                foreach (var field in filterFields.OrderBy(o => o.priority))
                {
                    filterBuilder.AddField(field, args.Filter);
                }
                filter = filterBuilder.ToString();
            }


            if (ForeginKey != null)
            {
                if (string.IsNullOrWhiteSpace(filter))
                {
                    filter = $"{ForeginKey.Name} eq {ForeginKey.Id}";
                }
                else
                {
                    filter = $"{filter} and {ForeginKey.Name} eq {ForeginKey.Id}";
                }
            }
            var queryParams = new QueryParams(args, ForeginKey);

            var result = await DataAdapter.GetAsync<TItem>(queryParams);
            //var result = await DataAdapter.GetAsync<TItem>(orderby: $"{args.OrderBy}",
                                               //top: args.Top,
                                               //skip: args.Skip,
                                               //count: args.Top != null && args.Skip != null,
                                               //expand: ExpandGrid,
                                               //filter: filter);
            dataCollection.Value = result.Value;
            dataCollection.Count = result.Count;
        }
        catch (Exception e)
        {
            string error = $"Помилка отримання даних {typeof(TItem).Name}  {e.Message}";
            NotificationService?.Notify(new NotificationMessage
            {
                Severity = NotificationSeverity.Error,
                Summary = "Помилка",
                Detail = error
            });
        }
        IsGridDataLoading = false;
    }

    
    protected int ColumnsPerPage
    {
        get => columnsPerPage;
        set
        {
            columnsPerPage = value;
            JSRuntime.InvokeVoidAsync("localStorage.setItem",PageName + nameof(ColumnsPerPage), value).GetAwaiter().GetResult();
        }
    }

    protected override void OnInitialized()
    {
        base.OnInitialized();
        //if (grid == null)
        //{
        //    NotificationService?.Notify(new NotificationMessage() { Severity = NotificationSeverity.Error, Summary = $"Error", Detail = $"НЕ призначено звязок для grid" });
        //    return;
        //}

        //columnsPerPage = LocalStorageService?.GetItem<int>(PageName + nameof(ColumnsPerPage)) ?? 10;
        //if (columnsPerPage == 0) 
        //{
        //    columnsPerPage = 10; 
        //}

    }

    protected override void OnAfterRender(bool firstRender)
    {
        base.OnAfterRender(firstRender);
        if (grid == null)
        {
            NotificationService?.Notify(new NotificationMessage() { Severity = NotificationSeverity.Error, Summary = $"Error", Detail = $"НЕ призначено звязок для grid" });
            return;
        }

        //columnsPerPage = JSRuntime.InvokeAsync<int?>("localStorage.getItem", PageName + nameof(ColumnsPerPage)).GetAwaiter().GetResult() ?? 10;
        //if (columnsPerPage == 0)
        //{
        //    columnsPerPage = 10;
        //}

    }
}
