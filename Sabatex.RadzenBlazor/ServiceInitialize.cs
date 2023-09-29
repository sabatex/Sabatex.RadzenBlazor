using Blazored.LocalStorage;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Radzen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sabatex.RadzenBlazor
{
    public static class ServiceInitialize
    {
        public static IServiceCollection AddSabatexRadzenBlazor(this IServiceCollection services) 
        {
            services.AddScoped<DialogService>();
            services.AddScoped<NotificationService>();
            services.AddScoped<TooltipService>();
            services.AddScoped<ContextMenuService>();
            services.AddScoped<sabatexJsInterop>();
            services.AddBlazoredLocalStorage();
            return services;
        }

        public static IServiceCollection AddSabatexRadzenBlazor<TDataAdapter>(this IServiceCollection services) where TDataAdapter : class, IRadzenDataAdapter
        {
            services.AddSabatexRadzenBlazor().AddScoped<IRadzenDataAdapter, TDataAdapter>();
            return services;
        }
    
    
        public static void AddNotNull(this QueryBuilder queryBuilder,  string key,string? value)
        {
            if (value != null) queryBuilder.Add(key, value);
        }
    
    }
}
