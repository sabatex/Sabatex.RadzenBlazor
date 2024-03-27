using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Radzen;
using Sabatex.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Sabatex.RadzenBlazor
{
    public abstract class SabatexRadzenBlazorBaseEditPage<TItem,TKey>:SabatexRadzenBlazorBaseDataPage<TKey> where TItem : class,IEntityBase<TKey>,new()
    {
        [Parameter]
        public string? Id { get; set; }

        [Parameter]
        [SupplyParameterFromQuery]
        public string? ReturnUrl { get; set; }

        protected TItem Item = new TItem();

        protected ValidationSummary? ValidationSummary;

        void ReturnToList()
        {
            if (ReturnUrl == null)
                NavigationManager.NavigateTo("/");
            else
            {
                //var uri = NavigationManager.GetUriWithQueryParameters(ReturnUrl,{});
                NavigationManager.NavigateTo(ReturnUrl);
            }

        }
        protected void Cancel()
        {
            ReturnToList();
        }
        //protected virtual async Task Save()
        //{
        //    if (Id == null)
        //        Item = await DataAdapter.PostAsync<TItem>(Item);
        //    else
        //        await DataAdapter.UpdateAsync<TItem>(Item);
        //    ReturnToList();
        //}

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            if (Id != null)
            {
                Item = await DataAdapter.GetByIdAsync<TItem>(Id);
            }
        }

        protected virtual async Task OnBeforeSubmit(TItem item) { await Task.Yield();}

        protected async Task OnSubmit(TItem item)
        {
            await OnBeforeSubmit(item);
            SabatexValidationModel<TItem>? result;

            try
            {
                if (Id == null)
                    result = await DataAdapter.PostAsync<TItem>(item);
                else
                    result = await DataAdapter.UpdateAsync<TItem>(Item);
                if (result == null)
                {
                   NotificationService?.Notify(new NotificationMessage() { Severity = NotificationSeverity.Error, Summary = $"Помилка запису", Detail = "Server response null" });
                    return;
                }

                if (result.Result != null)
                {
                    ReturnToList();
                    return;
                }
                
                if (result.Errors != null)
                    {
                        var error = result.Errors[""];
                        if (error != null)
                        {
                            string s = string.Empty;
                            foreach (var item1 in error) s = s + item1 + "\r\n";
                            NotificationService?.Notify(new NotificationMessage() { Severity = NotificationSeverity.Error, Summary = $"Помилка запису", Detail = s });
                        }

                        return;
                    }
                    NotificationService?.Notify(new NotificationMessage() { Severity = NotificationSeverity.Error, Summary = $"Помилка запису", Detail = "Uknown Error" });

 
            }
            catch (Exception ex) 
            {
                NotificationService?.Notify(new NotificationMessage() { Severity = NotificationSeverity.Error, Summary = $"Помилка запису", Detail = ex.Message });
            }
        }

    }
}
