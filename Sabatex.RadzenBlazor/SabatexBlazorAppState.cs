using System.Reflection.Metadata.Ecma335;
using Microsoft.AspNetCore.Components;

namespace Sabatex.RadzenBlazor;
public class SabatexBlazorAppState
{
       string pageHeader=string.Empty;
       public string PageHeader { get=>pageHeader; set { pageHeader = value; OnHeaderChange?.Invoke(value); } }
       public event Action<string>? OnHeaderChange;
    
       public Func<Task<double>> GetPageHeight;
       
       async Task<double> GetDoubleAsync()
       {
              await Task.Yield();
              return 100;

       }
       public SabatexBlazorAppState()
       {
              GetPageHeight = GetDoubleAsync;
       }

}