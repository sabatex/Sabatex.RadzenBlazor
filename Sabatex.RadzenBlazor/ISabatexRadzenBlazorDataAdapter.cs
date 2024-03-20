using Radzen;
using Sabatex.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sabatex.RadzenBlazor;

public interface ISabatexRadzenBlazorDataAdapter<TKey>
{
    Task<ODataServiceResult<TItem>> GetAsync<TItem>(string? filter, string? orderby, string? expand, int? top, int? skip, bool? count, string? format=null, string? select=null, string? apply = null) where TItem : class,IEntityBase<TKey>;
    Task<ODataServiceResult<TItem>> GetAsync<TItem>(QueryParams queryParams) where TItem : class, IEntityBase<TKey>;


    Task<TItem> GetByIdAsync<TItem>(TKey id, string? expand=null) where TItem: class, IEntityBase<TKey>;
    Task<TItem> GetByIdAsync<TItem>(string id, string? expand = null) where TItem : class, IEntityBase<TKey>;

    Task<TItem> PostAsync<TItem>(TItem? item) where TItem : class, IEntityBase<TKey>;
    Task DeleteAsync<TItem>(TKey id) where TItem : class, IEntityBase<TKey>;
    Task UpdateAsync<TItem>(TItem item) where TItem : class, IEntityBase<TKey>;
}

public record struct  FieldDescriptor(string Name,Type FieldType,string operation=" or ",int priority = 0,string? value=null);
//public record ForeginKey(string Name,string Id);
public class ForeginKey
{
    public string Name { get; set; }
    public string Id { get; set; }
    public ForeginKey(string name, string id)
    {
        Name = name;
        Id = id;
    }
    public ForeginKey()
    {
        
    }
}


