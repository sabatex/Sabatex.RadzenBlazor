namespace Sabatex.RadzenBlazor;
public class SabatexBlazorAppState
{
       string pageHeader=string.Empty;
       public string PageHeader { get=>pageHeader; set { pageHeader = value; OnHeaderChange?.Invoke(value); } }
       public event Action<string>? OnHeaderChange;
    
}