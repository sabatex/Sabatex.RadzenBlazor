using Radzen;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sabatex.Core;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http.Extensions;
using System.Net.Http.Json;
using System.Text.Encodings.Web;

namespace Sabatex.RadzenBlazor;

public class RadzenGRUDDataAdapter<TKey> : ISabatexRadzenBlazorDataAdapter<TKey>
{
     private readonly HttpClient _httpClient;
    public RadzenGRUDDataAdapter(HttpClient httpClient)
    {
        _httpClient = httpClient;

    }
    public async Task DeleteAsync<TItem>(TKey id) where TItem : class
    {
        if (id == null)
            throw new ArgumentNullException(nameof(id));
        var result = await _httpClient.DeleteAsync($"api/{typeof(TItem).Name}/{id}");
 
    }


    public async Task<ODataServiceResult<TItem>> GetAsync<TItem>(string? filter, string? orderby, string? expand, int? top, int? skip, bool? count, string? format = null, string? select = null) where TItem : class
    {
        var queryBuilder = new QueryBuilder();
        queryBuilder.AddNotNull("filter", filter);
        queryBuilder.AddNotNull("orderby", orderby);
        queryBuilder.AddNotNull("expand", expand);
        queryBuilder.AddNotNull("skip", skip?.ToString());
        queryBuilder.AddNotNull("count", count?.ToString());
        queryBuilder.AddNotNull("format", format?.ToString());
        queryBuilder.AddNotNull("select", format?.ToString());
        var query = $"api/{typeof(TItem).Name}{queryBuilder.ToQueryString()}";
        var responce = await _httpClient.GetAsync(query);
        return await Radzen.HttpResponseMessageExtensions.ReadAsync<Radzen.ODataServiceResult<TItem>>(responce);
    }

    public Task<TItem> GetByIdAsync<TItem>(TKey id, string? expand = null) where TItem : class
    {
        throw new NotImplementedException();
    }

    public Task<TItem> GetByIdAsync<TItem>(string id, string? expand = null) where TItem : class
    {
        throw new NotImplementedException();
    }

    public Task<TItem> PostAsync<TItem>(TItem? item) where TItem : class
    {
        throw new NotImplementedException();
    }

    public Task UpdateAsync<TItem>(TItem item) where TItem : EntityBase<TKey>
    {
        throw new NotImplementedException();
    }
}
