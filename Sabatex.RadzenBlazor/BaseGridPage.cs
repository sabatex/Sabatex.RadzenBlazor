using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Radzen;
using Radzen.Blazor;
using Sabatex.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sabatex.RadzenBlazor;

public class BaseGridPage<TItem>:BaseDataPage where TItem : class,IEntityBase,new()
{
    [Inject] IRadzenDataAdapter? _dataAdapter { get; set; }
    protected IRadzenDataAdapter DataAdapter => _dataAdapter != null ? _dataAdapter : throw new Exception("Data Adapter is not initialized");

    protected bool IsGridDataLoading = false;
    protected bool IsGridRO = false;
    protected RadzenDataGrid<TItem>? grid;
    protected RadzenODataCollection<TItem> dataCollection = new RadzenODataCollection<TItem>();
    protected virtual string? expandGrid => null;
    protected virtual ForeginKey? ForeginKey => null;
    protected async Task AddButtonClick(MouseEventArgs args)
    {
        if (grid == null)
        {
            notificationService?.Notify(new NotificationMessage() { Severity = NotificationSeverity.Error, Summary = $"Error", Detail = $"НЕ призначено звязок для grid" });
        }
        else
        {
            //await grid.InsertRow(itemToInsert);
        }
    }
    protected async Task gridLoadData(LoadDataArgs args)
    {
        IsGridDataLoading = true;
        try
        {
            await DataAdapter.FillData<TItem>(dataCollection, args, expandGrid, null, ForeginKey);
        }
        catch (Exception e)
        {
            string error = $"Помилка отримання даних {typeof(TItem).Name}  {e.Message}";
            notificationService?.Notify(new NotificationMessage
            {
                Severity = NotificationSeverity.Error,
                Summary = "Помилка",
                Detail = error
            });
        }
        IsGridDataLoading = false;
    }


}
