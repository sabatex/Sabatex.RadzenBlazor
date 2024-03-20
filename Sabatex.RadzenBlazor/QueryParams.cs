using Radzen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sabatex.RadzenBlazor;
//public record QueryParams(LoadDataArgs Args,ForeginKey? ForeginKey);

public class QueryParams
{
    public LoadDataArgs Args { get; set; }
    public ForeginKey? ForeginKey { get; set; }
    public QueryParams(LoadDataArgs args,ForeginKey? foreginKey = null)
    {
        Args = args; 
        ForeginKey = foreginKey;
    }
    public QueryParams()
    {
        Args = new LoadDataArgs();
    }
}