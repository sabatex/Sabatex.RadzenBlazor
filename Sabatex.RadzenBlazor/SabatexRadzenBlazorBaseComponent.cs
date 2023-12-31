﻿using Microsoft.AspNetCore.Components;
using Radzen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sabatex.RadzenBlazor
{
    public abstract class SabatexRadzenBlazorBaseComponent : RadzenComponent
    {
        [Inject]
        protected HttpClient HttpClient { get; set; } = default!;
        [Inject]
        protected NavigationManager NavigationManager { get; set; } = default!;
        [Inject]
        protected NotificationService NotificationService { get; set; } = default!;
        [Inject]
        protected Blazored.LocalStorage.ISyncLocalStorageService LocalStorageService { get; set; } = default!;

        [Inject]
        protected SabatexJsInterop SabatexJS {get;set;}=default!;

     }
}
