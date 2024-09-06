using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sabatex.RadzenBlazor
{
    public record PWAPushHandler(string endpoint,string p256dh,string auth);
    public interface IPWAPush
    {
        Task<PWAPushHandler?> GetSubscriptionAsync();
        Task<PWAPushHandler?> SubscribeAsync();
        Task UnSubscribeAsync();
        Task UpdateSubscriptionAsync();
        Task ClearSubscriptionAsync();

    }
}
