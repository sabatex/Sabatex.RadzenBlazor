using Microsoft.AspNetCore.Components;
using Radzen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sabatex.RadzenBlazor
{
    public abstract class SabatexRadzenBlazorBasePage : SabatexRadzenBlazorBaseComponent
    {
        [Inject]
        protected SabatexBlazorAppState AppState {get;set;}=default!;

        protected string PageName { get => AppState.PageHeader; set => AppState.PageHeader = value; }

    }
}
