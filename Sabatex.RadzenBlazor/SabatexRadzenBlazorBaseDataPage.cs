using Microsoft.AspNetCore.Components;
using Radzen;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Xml.Linq;


namespace Sabatex.RadzenBlazor;

public abstract class SabatexRadzenBlazorBaseDataPage<TKey>: SabatexRadzenBlazorBasePage 
{
    protected bool IsPageRO = false;
    [Inject]
    protected ISabatexRadzenBlazorDataAdapter<TKey> DataAdapter { get; set; } = default!;
   
}
