using Blazored.LocalStorage;
using Microsoft.Extensions.DependencyInjection;
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
            services.AddBlazoredLocalStorage();
            return services;
        }
    }
}
