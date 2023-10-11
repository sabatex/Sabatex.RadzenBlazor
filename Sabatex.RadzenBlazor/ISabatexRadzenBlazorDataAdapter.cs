using Radzen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sabatex.RadzenBlazor;

public interface ISabatexRadzenBlazorDataAdapter
{
    Task<(IEnumerable<TItem> items, int count)> GetAsync<TItem>(string? filter, string? orderby, string? expand, int? top, int? skip, bool? count, string? format=null, string? select=null) where TItem : class;
    Task<TItem> GetByIdAsync<TItem,TKey>(TKey id, string? expand=null) where TItem:class;
    Task FillData<TItem>(RadzenODataCollection<TItem>? dataCollection, LoadDataArgs args, string? expand=null,IEnumerable<FieldDescriptor>? filterFields = null, ForeginKey? foreginKey = null) where TItem : class;

    Task<TItem> PostAsync<TItem>(TItem? item) where TItem : class;
    Task DeleteAsync<TItem>(string id) where TItem : class;
    Task UpdateAsync<TItem>(string id, TItem item) where TItem : class;
    void ExportToExcel<TItem>(Radzen.Query? query = null, string? fileName = null) where TItem : class;
    void ExportToCSV<TItem>(Radzen.Query? query = null, string? fileName = null) where TItem : class;
}

public record struct  FieldDescriptor(string Name,Type FieldType,string operation=" or ",int priority = 0,string? value=null);
public record ForeginKey(string Name,string Id);
