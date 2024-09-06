using Sabatex.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sabatex.RadzenBlazor.Models;

public class NotificationSubscription: IEntityBase<Guid>
{
    public Guid Id { get; set; }

    public string? UserId { get; set; }

    public string? Endpoint { get; set; }

    public string? P256dh { get; set; }

    public string? Auth { get; set; }

}