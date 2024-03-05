using Microsoft.AspNetCore.Components;
using Sabatex.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sabatex.RadzenBlazor
{
    public abstract class SabatexRadzenBlazorBaseEditPage<TItem,TKey>:SabatexRadzenBlazorBaseDataPage<TKey> where TItem : IEntityBase<TKey>,new()
    {
        [Parameter]
        public string? Id { get; set; }

        [Parameter]
        [SupplyParameterFromQuery]
        public string? ReturnUrl { get; set; }

        protected TItem Item = new TItem();


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
        protected virtual async Task Save()
        {
            if (Id == null)
                Item = await DataAdapter.PostAsync<TItem>(Item);
            else
                await DataAdapter.UpdateAsync<TItem>(Item);
            ReturnToList();
        }

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            if (Id != null)
            {
                Item = await DataAdapter.GetByIdAsync<TItem>(Id);
            }
        }




    }
}
