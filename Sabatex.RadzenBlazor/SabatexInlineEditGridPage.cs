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

public abstract class SabatexInlineEditGridPage<TItem,TKey> : SabatexRadzenBlazorBaseGridPage<TItem,TKey> where TItem :EntityBase<TKey>, new()
{
    public async Task Reload()
    {
        await InvokeAsync(StateHasChanged);
    }


    protected bool isGridDataLoading = false;
    
    protected virtual string? expandGrid => null;
    protected virtual string? filterGrid => null;
    protected virtual IEnumerable<FieldDescriptor>? filterFields=>null;

    protected ODataServiceResult<TItem> dataCollection=new ODataServiceResult<TItem>();

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

    protected virtual void OnCreatedRow(TItem item) { }
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
        await DataAdapter.UpdateAsync(item);
        itemToUpdate = null;
    }
    protected async Task Load()
    {
        var item  = new TItem();
        await Task.Yield();
    }


    protected override async Task AddButtonClick(MouseEventArgs args)
    {
            itemToInsert = new TItem();
            OnCreatedRow(itemToInsert);
            if (dataCollection.Count == 0) dataCollection.Count++;
            await grid.InsertRow(itemToInsert);
    }

    protected override async Task EditButtonClick(TItem data)
    {
            await grid.EditRow(data);
            itemToUpdate = data;
    }
    
    protected virtual void OnBeforeSave(TItem data) {}
    protected async Task SaveButtonClick(TItem data)
    {
        OnBeforeSave(data);
        await grid.UpdateRow(data);
    }
    protected async Task CancelButtonClick(TItem data)
    {
        grid.CancelEditRow(data);
        await this.Load();
        await grid.Reload();
        itemToInsert = null;
        itemToUpdate= null;
    }


}
