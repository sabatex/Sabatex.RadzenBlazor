using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using Radzen;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace Sabatex.RadzenBlazor;

public class RadzenOdataAdapter : IRadzenDataAdapter
{
    private readonly HttpClient httpClient;
    private readonly Uri baseUri;
    private readonly NavigationManager navigationManager;
    private readonly NotificationService notificationService;
    private readonly ILogger<RadzenOdataAdapter> logger;


    public RadzenOdataAdapter(HttpClient httpClient, NavigationManager navigationManager, NotificationService notificationService,ILogger<RadzenOdataAdapter> logger)
    {
        this.httpClient = httpClient;
        this.navigationManager = navigationManager;
        baseUri = new Uri(this.httpClient.BaseAddress ?? new Uri("/"),"odata/");
        this.notificationService = notificationService;
        this.logger= logger;
    }
    public async Task<(IEnumerable<TItem> items,int count)> GetAsync<TItem>(string? filter, string? orderby, string? expand, int? top, int? skip, bool? count, string? format = null, string? select = null) where TItem : class
    {
        try
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
            //body = Radzen.ODataExtensions.GetODataUri(uri: body, filter: filter, top: top, skip: skip, orderby: orderby, expand: expand, select: select, count: count);
  
            //var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, uri);
            var response = await httpClient.PostAsync(uri.ToString(), new StringContent(query.ToString(), Encoding.UTF8, "text/plain"));
            var result = await Radzen.HttpResponseMessageExtensions.ReadAsync<Radzen.ODataServiceResult<TItem>>(response);
            return (result.Value.AsODataEnumerable(), result.Count);
        }
        catch (Exception ex)
        {
            string error = $"Error method RadzenOdataAdapter.GET<{typeof(TItem).Name}>() -> {ex.Message}";
            throw new Exception(error);
        }
    }

    public async Task<TItem> PostAsync<TItem>(TItem? item) where TItem : class
    {
        var uri = new Uri(baseUri, typeof(TItem).Name);
        var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, uri);
        httpRequestMessage.Content = new StringContent(Radzen.ODataJsonSerializer.Serialize(item), Encoding.UTF8, "application/json");
        var response = await httpClient.SendAsync(httpRequestMessage);
        return await Radzen.HttpResponseMessageExtensions.ReadAsync<TItem>(response);
    }
    public async Task DeleteAsync<TItem>(string id) where TItem : class
    {
        try
        {
            var uri = new Uri(baseUri, $"{typeof(TItem).Name}({id})"); ;
            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Delete, uri);
            var responce = await httpClient.SendAsync(httpRequestMessage);
            if (responce?.StatusCode == System.Net.HttpStatusCode.NoContent)
                    return;
             if (responce == null)
                throw new Exception("Невідома помилка при видаленні");
            throw new Exception($"Код відповіді сервера {responce.StatusCode}");
        }
        catch (Exception e)
        {
            string error = $"RadzenOdataAdapter помилка видалення з Entity {typeof(TItem).Name} значення з Id={id}. Помилка: {e.Message}";
            throw new Exception(error);
        }
    }
    public async Task UpdateAsync<TItem>(string id, TItem item) where TItem : class
    {
        try
        {
            var uri = new Uri(baseUri, $"{typeof(TItem).Name}({id})");
            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Patch, uri);
            httpRequestMessage.Content = new StringContent(Radzen.ODataJsonSerializer.Serialize(item), Encoding.UTF8, "application/json");
            var responce = await httpClient.SendAsync(httpRequestMessage);
            if (responce == null)
                throw new Exception("Невідома помилка");    
            if (responce.StatusCode == System.Net.HttpStatusCode.NotFound)
                throw new Exception($"Відсутній запис для Entity<{typeof(TItem).Name}> з Id = {id}");
            if (responce.StatusCode == System.Net.HttpStatusCode.BadRequest)
                throw new Exception($"Код відповіді сервера - BadRequest");
        }
        catch (Exception e)
        {
            throw new Exception($"Даны не оновлено! Error:{e.Message}");
         }
        
    }

    public void ExportToExcel<TItem>(Radzen.Query? query = null, string? fileName = null) where TItem : class
    {
        navigationManager.NavigateTo(query != null ? query.ToUrl($"api/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/tg/tgclients/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
    }
    public void ExportToCSV<TItem>(Radzen.Query? query = null, string? fileName = null) where TItem : class
    {
        navigationManager.NavigateTo(query != null ? query.ToUrl($"api/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/tg/tgclients/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
    }

    public async Task FillData<TItem>(RadzenODataCollection<TItem>? dataCollection, LoadDataArgs args, string? expand = null, IEnumerable<FieldDescriptor>? filterFields = null,ForeginKey? foreginKey=null ) where TItem : class
    {
        try
        {

            if (dataCollection == null)
                throw new ArgumentNullException(nameof(dataCollection));
            string filter = string.Empty;
            if (filterFields != null)
            {
                // search in combobox
            }
            else
                filter = args.Filter;
            
            if (filterFields != null && !string.IsNullOrWhiteSpace(args.Filter))
            {
                ODataSearchFilterBuilder filterBuilder = new ODataSearchFilterBuilder();
                foreach (var field in filterFields.OrderBy(o => o.priority))
                {
                    filterBuilder.AddField(field, args.Filter);
                }
                filter = filterBuilder.ToString();
            }


            if (foreginKey!= null)
            {
                if (string.IsNullOrWhiteSpace(filter))
                {
                    filter = $"{foreginKey.Name} eq {foreginKey.Id}";
                }
                else
                {
                    filter = $"{filter} and {foreginKey.Name} eq {foreginKey.Id}";
                }
            }

            var result = await GetAsync<TItem>(orderby: $"{args.OrderBy}",
                                               top: args.Top,
                                               skip: args.Skip,
                                               count: args.Top != null && args.Skip != null,
                                               expand: expand,
                                               filter: filter);
            dataCollection.Items = result.items;
            dataCollection.Count = result.count;
        }catch(Exception ex)
        {
            string error = $"Помилка зчитування {typeof(TItem).Name} \r\n {ex.Message}";
            notificationService?.Notify(new NotificationMessage
            {
                Severity = NotificationSeverity.Error,
                Summary = "Error",
                Detail = error
            });

        }

    }

 
    public async Task<TItem> GetByIdAsync<TItem, TKey>(TKey id, string? expand=null) where TItem : class
    {
        try
        {
            var uri = new Uri(baseUri, $"{typeof(TItem).Name}({id})");
            uri = Radzen.ODataExtensions.GetODataUri(uri: uri, filter: null, top: null, skip: null, orderby: null, expand: expand, select: null, count: null);
            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, uri);
            var response = await httpClient.SendAsync(httpRequestMessage);
            return await Radzen.HttpResponseMessageExtensions.ReadAsync<TItem>(response);
        }
        catch (Exception ex)
        {
            string error = $"Error method RadzenOdataAdapter.GetByIdAsync<{typeof(TItem).Name}>() -> {ex.Message}";
            throw new Exception(error);
        }

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

    void AddStringSearch(string FieldName,string value)
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
    public void AddField(FieldDescriptor fieldDescriptor,string value)
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