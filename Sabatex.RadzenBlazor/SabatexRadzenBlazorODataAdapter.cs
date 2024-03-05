using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using Radzen;
using Sabatex.Core;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace Sabatex.RadzenBlazor;

public class SabatexRadzenBlazorODataAdapter<TKey> : ISabatexRadzenBlazorDataAdapter<TKey>
{
    const string nullResponce = "The responece return null";

    private readonly HttpClient httpClient;
    private readonly Uri baseUri;
    private readonly ILogger<SabatexRadzenBlazorODataAdapter<TKey>> logger;



    public SabatexRadzenBlazorODataAdapter(HttpClient httpClient, ILogger<SabatexRadzenBlazorODataAdapter<TKey>> logger)
    {
        this.httpClient = httpClient;
        baseUri = new Uri(this.httpClient.BaseAddress ?? new Uri("/"), "odata/");
        this.logger = logger;
    }
    public async Task<ODataServiceResult<TItem>> GetAsync<TItem>(string? filter, string? orderby, string? expand, int? top, int? skip, bool? count, string? format = null, string? select = null) where TItem : IEntityBase<TKey>
    {
            var uri = new Uri(baseUri, $"{typeof(TItem).Name}/$query");

            var query = new StringBuilder();
            bool last = false;
            if (top.HasValue)
            {
                query.Append($"$top={top}");
                last = true;
            }
            if (skip.HasValue)
            {
                if (last) query.Append('&');
                query.Append($"$skip={skip}");
                last = true;

            }
            if (count ?? false)
            {
                if (last) query.Append('&');
                query.Append($"$count=true");
                last = true;
            }
            if (!string.IsNullOrEmpty(orderby))
            {
                if (last) query.Append('&');
                query.Append($"$orderby={orderby}");
                last = true;
            }
            if (!string.IsNullOrEmpty(filter))
            {
                if (last) query.Append('&');
                query.Append($"$filter={filter}");
                last = true;
            }
            if (!string.IsNullOrEmpty(expand))
            {
                if (last) query.Append('&');
                query.Append($"$expand={expand}");
                last = true;
            }
            if (!string.IsNullOrEmpty(select))
            {
                if (last) query.Append('&');
                query.Append($"$select={select}");
                last = true;
            }
            logger.LogInformation(query.ToString());

            var response = await httpClient.PostAsync(uri.ToString(), new StringContent(query.ToString(), Encoding.UTF8, "text/plain"));
            return await Radzen.HttpResponseMessageExtensions.ReadAsync<Radzen.ODataServiceResult<TItem>>(response);
 
    }

    public async Task<TItem> PostAsync<TItem>(TItem? item) where TItem : IEntityBase<TKey>
    {
        var uri = new Uri(baseUri, typeof(TItem).Name);
        var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, uri);
        httpRequestMessage.Content = new StringContent(Radzen.ODataJsonSerializer.Serialize(item), Encoding.UTF8, "application/json");
        var response = await httpClient.SendAsync(httpRequestMessage);
        return await Radzen.HttpResponseMessageExtensions.ReadAsync<TItem>(response);
    }
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TItem"></typeparam>
    /// <param name="id"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public async Task DeleteAsync<TItem>(TKey id) where TItem : IEntityBase<TKey>
    {
        var uri = new Uri(baseUri, $"{typeof(TItem).Name}({id})"); ;
        var httpRequestMessage = new HttpRequestMessage(HttpMethod.Delete, uri);
        var responce = await httpClient.SendAsync(httpRequestMessage);
        if (responce == null)
            throw new Exception(nullResponce);

        if (responce.StatusCode != System.Net.HttpStatusCode.NoContent)
            throw new Exception($"Delete error with responce code = {responce.StatusCode}");
    }
    public async Task UpdateAsync<TItem>(TItem item) where TItem : IEntityBase<TKey>
    {
        var uri = new Uri(baseUri, $"{typeof(TItem).Name}({item.Id})");
        var httpRequestMessage = new HttpRequestMessage(HttpMethod.Patch, uri);
        httpRequestMessage.Content = new StringContent(Radzen.ODataJsonSerializer.Serialize(item), Encoding.UTF8, "application/json");
        var responce = await httpClient.SendAsync(httpRequestMessage);
        if (responce == null)
                throw new Exception(nullResponce);
        if (responce.StatusCode == System.Net.HttpStatusCode.NotFound)
                throw new Exception($"Відсутній запис для Entity<{typeof(TItem).Name}> з Id = {item.Id}");
        if (responce.StatusCode == System.Net.HttpStatusCode.BadRequest)
                throw new Exception($"Код відповіді сервера - BadRequest");
    }

    public async Task<TItem> GetByIdAsync<TItem>(TKey id, string? expand = null) where TItem : IEntityBase<TKey>
    {
        if (id is not null)
            return await GetByIdAsync<TItem>(id.ToString(),expand);
        else
            throw new ArgumentNullException(nameof(id));
    }

    public async Task<TItem> GetByIdAsync<TItem>(string? id, string? expand = null) where TItem : IEntityBase<TKey>
    {
        var uri = new Uri(baseUri, $"{typeof(TItem).Name}({id})");
        uri = Radzen.ODataExtensions.GetODataUri(uri: uri, filter: null, top: null, skip: null, orderby: null, expand: expand, select: null, count: null);
        var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, uri);
        var response = await httpClient.SendAsync(httpRequestMessage);
        return await Radzen.HttpResponseMessageExtensions.ReadAsync<TItem>(response);

    }
}

internal struct ODataSearchFilterBuilder
{
    StringBuilder sb;
    bool first;
    public ODataSearchFilterBuilder()
    {
        sb = new StringBuilder();
        first = true;

    }

    void AddStringSearch(string FieldName, string value)
    {
        AddOperation();
        sb.Append($"contains(tolower({FieldName}),'{value}')");
    }
    void AddIntSearch(string FieldName, string value)
    {
        if (int.TryParse(value, out int _))
        {
            AddOperation();
            sb.Append($"{FieldName} eq {value}");
        }
    }
    void AddOperation()
    {
        if (!first) sb.Append(" or ");
        first = false;
    }
    public void AddField(FieldDescriptor fieldDescriptor, string value)
    {
        if (fieldDescriptor.FieldType == typeof(string))
        {
            AddStringSearch(fieldDescriptor.Name, value);
        }
        else if (fieldDescriptor.FieldType == typeof(int))
        {
            AddIntSearch(fieldDescriptor.Name, value);
        }


    }
    public override string ToString()
    {
        return sb.ToString();
    }
}