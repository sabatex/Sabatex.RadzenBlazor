using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Radzen;
using Radzen.Blazor;
using Sabatex.Core;
using System;
using System.ComponentModel;
using System.Net.Http;
using System.Net.Sockets;
using System.Runtime.InteropServices;


namespace Sabatex.RadzenBlazor;

public abstract class InlineEditGridPage<TItem> : BaseDataPage where TItem :class,IEntityBase, new()
{
    public async Task Reload()
    {
        await InvokeAsync(StateHasChanged);
    }
   
    [Inject] protected DialogService? DialogService { get; set; }

    [Inject] protected IRadzenDataAdapter? dataAdapter { get; set; }
    protected IRadzenDataAdapter DataAdapter =>dataAdapter!=null?dataAdapter:throw new Exception("Data Adapter is not initialized");

    protected bool isGridDataLoading = false;
    protected RadzenDataGrid<TItem>? grid;
    
    protected virtual string? expandGrid => null;
    protected virtual string? filterGrid => null;

    protected RadzenODataCollection<TItem> dataCollection=new RadzenODataCollection<TItem>();

    TItem? itemToInsert;
    TItem? itemToUpdate;


    protected bool IsGridBusy=>itemToInsert != null || itemToUpdate!= null;


    private void GridRefNull()
    {
            NotificationService?.Notify(new NotificationMessage
            {
                Severity = NotificationSeverity.Error,
                Summary = "Error",
                Detail = "The grid reference not initialize!"
            });
    }
    private async Task GridReload()
    {
        if (grid != null)
        {
            await grid.Reload();
        }
        else
        {
            GridRefNull();
        }
    }

    protected virtual void OnCreatedRow(TItem item) { }

    protected async Task AddButtonClick(MouseEventArgs args)
    {
        if (grid == null)
        {
            NotificationService?.Notify(new NotificationMessage() { Severity = NotificationSeverity.Error, Summary = $"Error", Detail = $"НЕ призначено звязок для grid" });
        }
        else 
        {
            itemToInsert = new TItem();
            OnCreatedRow(itemToInsert);
            if (dataCollection.Count == 0) dataCollection.Count++;
            await grid.InsertRow(itemToInsert);
        }
    }

    protected virtual ForeginKey? ForeginKey => null;

    protected async Task gridLoadData(LoadDataArgs args)
    {
        isGridDataLoading = true;
        try
        {
              await DataAdapter.FillData<TItem>(dataCollection, args,expandGrid,null,ForeginKey);
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
        isGridDataLoading = false;
    }

    protected async Task gridRowCreate(TItem item)
    {
        var result = await DataAdapter.PostAsync(item);
        if (grid != null)
            await grid.Reload();
        itemToInsert = null;
        await InvokeAsync(() => { StateHasChanged(); });
    }
    protected async Task gridRowUpdate(TItem item)
    {
        await DataAdapter.UpdateAsync(item.KeyAsString(), item);
        itemToUpdate = null;
    }
    protected async Task Load()
    {
        //var tgGetTgClientCategoriesResult = await Tg.GetTgClientCategories();
        //getTgClientCategoriesResult = tgGetTgClientCategoriesResult.Value.AsODataEnumerable();

        //var tgGetTgRegionsResult = await Tg.GetTgRegions();
        //getTgRegionsResult = tgGetTgRegionsResult.Value.AsODataEnumerable();

        var item  = new TItem();
        await Task.Yield();
    }

    protected async Task GridDeleteButtonClick(TItem data)
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
                    await DataAdapter.DeleteAsync<TItem>(data.KeyAsString());
                    await GridReload();
                }
                catch(Exception e)
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
    protected async Task EditButtonClick(TItem data)
    {
        if (grid == null)
        {
            GridRefNull();
        }
        else
        {
            await grid.EditRow(data);
            itemToUpdate = data;
        }
    }
    
    protected virtual void OnBeforeSave(TItem data) {}
    protected async Task SaveButtonClick(TItem data)
    {
        if (grid == null)
        {
            GridRefNull();
        }
        else
        {
            OnBeforeSave(data);
            await grid.UpdateRow(data);
        }
    }
    protected async Task CancelButtonClick(TItem data)
    {
        grid?.CancelEditRow(data);
        await this.Load();
        await GridReload();
        itemToInsert = null;
        itemToUpdate= null;
    }

    protected override void OnInitialized()
    {
        base.OnInitialized();
        dataCollection = new RadzenODataCollection<TItem>();
    }
}
