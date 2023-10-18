using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
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

public class SabatexRadzenBlazorBaseGridPage<TItem, TKey> : SabatexRadzenBlazorBaseDataPage<TKey> where TItem : EntityBase<TKey> 
{
    [Inject] protected DialogService DialogService { get; set; } = default!;

    protected bool IsGridDataLoading = false;
    protected bool IsGridRO = false;
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
            return $"{baseRoute}{typeof(TItem).Name}Edit";
        }
    }

    private void NavigateToEditPage(string? id)
    {
        string? idRoute = string.Empty;
        if (id != null)
            idRoute = $"/{id}";


        var queryParams = new Dictionary<string, object?>()
        {
            ["returnUrl"] = NavigationManager.ToBaseRelativePath(NavigationManager.Uri)
        };
 
        var uri = NavigationManager.GetUriWithQueryParameters($"{EditPageUri}{idRoute}", queryParams);
 
        NavigationManager.NavigateTo(uri);
    }
    protected virtual async Task AddButtonClick(MouseEventArgs args)
    {
        NavigateToEditPage(null);
        await Task.Yield();
    }
    protected virtual async Task RowDoubleClick(DataGridRowMouseEventArgs<TItem> args)
    {
        NavigateToEditPage(args.Data.Id.ToString());
        await Task.Yield();
    }
    protected virtual async Task EditButtonClick(TItem data)
    {
        NavigateToEditPage(data.Id.ToString());
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

            var result = await DataAdapter.GetAsync<TItem>(orderby: $"{args.OrderBy}",
                                               top: args.Top,
                                               skip: args.Skip,
                                               count: args.Top != null && args.Skip != null,
                                               expand: ExpandGrid,
                                               filter: filter);
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
            LocalStorageService?.SetItem(PageName + nameof(ColumnsPerPage), value);
        }
    }

    protected override void OnInitialized()
    {
        base.OnInitialized();
        if (grid == null)
        {
            NotificationService?.Notify(new NotificationMessage() { Severity = NotificationSeverity.Error, Summary = $"Error", Detail = $"НЕ призначено звязок для grid" });
            return;
        }

        columnsPerPage = LocalStorageService?.GetItem<int>(PageName + nameof(ColumnsPerPage)) ?? 10;
        if (columnsPerPage == 0) 
        {
            columnsPerPage = 10; 
        }

    }
}
