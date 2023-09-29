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

namespace Sabatex.RadzenBlazor;

public class RadzenGRUDDataAdapter<TKey> : IRadzenDataAdapter
{
    private readonly NotificationService _notificationService;
    private readonly HttpClient _httpClient;
    public RadzenGRUDDataAdapter(NotificationService notificationService, HttpClient httpClient)
    {
        _notificationService = notificationService;
        _httpClient = httpClient;

    }
    public async Task DeleteAsync<TItem>(TKey id) where TItem : class,IEntityBase
    {
        if (id == null)
            throw new ArgumentNullException(nameof(id));
        try
        {
            var result = await _httpClient.DeleteAsync($"api/{typeof(TItem).Name}/{id}");
        }
        catch (HttpRequestException exception)
        {
            
        }
    }

    public Task DeleteAsync<TItem>(string id) where TItem : class
    {
        throw new NotImplementedException();
    }

    public void ExportToCSV<TItem>(Query? query = null, string? fileName = null) where TItem : class
    {
        throw new NotImplementedException();
    }

    public void ExportToExcel<TItem>(Query? query = null, string? fileName = null) where TItem : class
    {
        throw new NotImplementedException();
    }

    public async Task FillData<TItem>(RadzenODataCollection<TItem>? dataCollection, LoadDataArgs args, string? expand = null, IEnumerable<FieldDescriptor>? filterFields = null, ForeginKey? foreginKey = null) where TItem : class
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


            if (foreginKey != null)
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
        }
        catch (Exception ex)
        {
            string error = $"Помилка зчитування {typeof(TItem).Name} \r\n {ex.Message}";
            _notificationService?.Notify(new NotificationMessage
            {
                Severity = NotificationSeverity.Error,
                Summary = "Error",
                Detail = error
            });

        }


    }

    public async Task<(IEnumerable<TItem> items, int count)> GetAsync<TItem>(string? filter, string? orderby, string? expand, int? top, int? skip, bool? count, string? format = null, string? select = null) where TItem : class
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
        try
        {
            var result = await _httpClient.GetFromJsonAsync<RadzenODataCollection<TItem>>(query);
            if (result == null)
                throw new Exception();
            return (result.Items ?? new TItem[] { },result.Count);
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }

    }

    public Task<TItem> GetByIdAsync<TItem>(TKey id, string? expand = null) where TItem : class
    {
        throw new NotImplementedException();
    }

    public Task<TItem> GetByIdAsync<TItem, TKey1>(TKey1 id, string? expand = null) where TItem : class
    {
        throw new NotImplementedException();
    }

    public Task<TItem> PostAsync<TItem>(TItem? item) where TItem : class
    {
        throw new NotImplementedException();
    }

    public Task UpdateAsync<TItem>(string id, TItem item) where TItem : class
    {
        throw new NotImplementedException();
    }
}
