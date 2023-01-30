using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sabatex.RadzenBlazor;

public class RadzenODataCollection<TItem> where TItem : class
{
    /// <summary>
    /// Count all collection
    /// </summary>
    public int Count { get; set; }
    /// <summary>
    /// part collection
    /// </summary>
    public IEnumerable<TItem>? Items { get; set; }
}
