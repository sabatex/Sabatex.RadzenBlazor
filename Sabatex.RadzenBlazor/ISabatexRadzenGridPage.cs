using Microsoft.AspNetCore.Components.Web;

namespace Sabatex.RadzenBlazor
{
    public interface ISabatexRadzenGridPage{
        public Task AddButtonClick(MouseEventArgs args);
        public bool IsGridRO{get;set;}
    }    
}