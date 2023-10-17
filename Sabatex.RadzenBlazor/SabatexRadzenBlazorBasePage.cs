using Microsoft.AspNetCore.Components;
using Radzen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sabatex.RadzenBlazor
{
    public abstract class SabatexRadzenBlazorBasePage : ComponentBase
    {
        [Inject]
        protected HttpClient HttpClient { get; set; } = default!;
        [Inject]
        protected NavigationManager NavigationManager { get; set; } = default!;
        [Inject]
        protected NotificationService NotificationService { get; set; } = default!;
        [Inject]
        protected Blazored.LocalStorage.ISyncLocalStorageService LocalStorageService { get; set; } = default!;

        protected string PageName { get; set; } = "Default";
    }
}
