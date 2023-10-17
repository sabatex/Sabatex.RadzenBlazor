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
    Task<ODataServiceResult<TItem>> GetAsync<TItem>(string? filter, string? orderby, string? expand, int? top, int? skip, bool? count, string? format=null, string? select=null) where TItem : class;
    Task<TItem> GetByIdAsync<TItem>(TKey id, string? expand=null) where TItem:class;
    Task<TItem> GetByIdAsync<TItem>(string id, string? expand = null) where TItem : class;

    Task<TItem> PostAsync<TItem>(TItem? item) where TItem : class;
    Task DeleteAsync<TItem>(TKey id) where TItem : class;
    Task UpdateAsync<TItem>(TItem item) where TItem : EntityBase<TKey>;
}

public record struct  FieldDescriptor(string Name,Type FieldType,string operation=" or ",int priority = 0,string? value=null);
public record ForeginKey(string Name,string Id);
