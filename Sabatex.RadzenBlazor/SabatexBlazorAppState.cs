using System.Reflection.Metadata.Ecma335;
using Microsoft.AspNetCore.Components;

namespace Sabatex.RadzenBlazor;
public class SabatexBlazorAppState
{
       string pageHeader = string.Empty;
       public string PageHeader { get => pageHeader; set { pageHeader = value; OnHeaderChange?.Invoke(value); } }
       public event Action<string>? OnHeaderChange;

       double contentAvaliableHeight;
       public event Action<double>? OnContentAvaliableHeightChange;
       public double ContentAvaliableHeight
       {
              get => contentAvaliableHeight;
              set
              {
                     if (contentAvaliableHeight != value)
                     {
                            contentAvaliableHeight = value;
                            OnContentAvaliableHeightChange?.Invoke(value);
                     }
              }
       }


}