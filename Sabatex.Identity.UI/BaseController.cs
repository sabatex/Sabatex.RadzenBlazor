using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Radzen;
using Sabatex.Core;
using Sabatex.RadzenBlazor;
using System.ComponentModel;
using System.Linq.Dynamic.Core;
using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;


namespace Sabatex.Identity.UI;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public abstract class BaseController<TItem> : ControllerBase where TItem : class, IEntityBase<Guid>, new()
{
    protected readonly DbContext context;
    protected readonly ILogger logger;

    protected BaseController(DbContext context, ILogger logger)
    {
        this.context = context;
        this.logger = logger;
    }


    public Guid UserId => Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty);
    protected virtual IQueryable<TItem> OnAfterIncludeInGet(IQueryable<TItem> query, QueryParams queryParams)
    {
        return query;
    }
    protected virtual IQueryable<TItem> OnAfterWhereInGet(IQueryable<TItem> query, QueryParams queryParams)
    {
        return query;
    }



    [HttpGet]
    public virtual async Task<ODataServiceResult<TItem>> Get([FromQuery] string json)
    {
        QueryParams? queryParams = JsonSerializer.Deserialize<QueryParams>(Uri.UnescapeDataString(json));

        if (queryParams == null)
            throw new Exception("Deserialize error");

        var query = context.Set<TItem>().AsQueryable<TItem>();
        if (queryParams.Include != null)
        {
            foreach (var item in queryParams.Include)
           {
                query = query.Include(item);
           }
        }
        query = OnAfterIncludeInGet(query, queryParams);

        if (queryParams.ForeginKey != null)
        {
            query = query.Where($"it => it.{queryParams.ForeginKey.Name}.ToString() == \"{queryParams.ForeginKey.Id}\"");
        }

        if (!String.IsNullOrEmpty(queryParams.Args.Filter))
            query = query.Where(queryParams.Args.Filter); 

        query = OnAfterWhereInGet(query,queryParams);

        if (!String.IsNullOrEmpty(queryParams.Args.OrderBy))
        {
            query = query.OrderBy(queryParams.Args.OrderBy);
        }
       
        if (queryParams.Args.Skip != null)
            query = query.Skip(queryParams.Args.Skip.Value); 
        if (queryParams.Args.Top != null)
            query = query.Take(queryParams.Args.Top.Value);

        var result = new ODataServiceResult<TItem>();
        result.Value = await query.ToArrayAsync();
        if ((queryParams.Args.Skip != null) || (queryParams.Args.Top != null))
            result.Count = await query.CountAsync();
        return result;
    }
 
    
    protected virtual IQueryable<TItem> OnBeforeGetById(IQueryable<TItem> query,Guid id)
    {
        return query;
    }

    protected virtual async Task OnAfterGetById(TItem item, Guid id)
    {
        await Task.Yield();
    }

    protected abstract Task<bool> CheckAccess(TItem item,TItem? updated);


    [HttpGet("{id}")]
    public virtual async Task<IActionResult> GetById([FromRoute]Guid id)
    {
        var query = context.Set<TItem>().AsQueryable<TItem>();
        query = OnBeforeGetById(query,id);
        var result  = await query.Where(s=>s.Id == id).SingleAsync();
        if (await CheckAccess(result,null))
        {
            await OnAfterGetById(result, id);
            return Ok(result);
        }
        return Unauthorized(); 
    }


    protected virtual async Task OnBeforeAddItemToDatabase(TItem item) => await Task.Yield();

    [HttpPost]
    public virtual async Task<IActionResult> Post([FromBody] TItem value)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        if (value == null)
        {
            ModelState.AddModelError(string.Empty, "The post null value");
            return BadRequest(ModelState);
        }
        try
        {
            await this.OnBeforeAddItemToDatabase(value);
            if (await CheckAccess(value, null))
            {
                await context.Set<TItem>().AddAsync(value);
                await context.SaveChangesAsync();
                return Ok(value);
            }
            return Unauthorized();
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", ex.Message);
            return BadRequest(ModelState);
        }
    }
    
    protected virtual async Task OnBeforeUpdateAsync(TItem item,TItem update)
    {
       await Task.Yield();
    }
    [HttpPut("{id}")]
    public virtual async Task<IActionResult> Put([FromRoute] Guid id, TItem update)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        if (id != update.Id)
        {
            return BadRequest();
        }

        var item = await context.Set<TItem>().FindAsync(id);
        if (item == null)
            return NotFound();

        if (!await CheckAccess(item,update))
            return Unauthorized(ModelState);
        
        await OnBeforeUpdateAsync(item,update);
        context.Entry(item).CurrentValues.SetValues(update);
        try
        {
            await context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!ValueExists(id))
            {
                return NotFound();
            }
            else
            {
                throw;
            }
        }
        return Ok(update);
    }

    [HttpDelete("{id}")]
    public virtual async Task<IActionResult> Delete([FromRoute] Guid id)
    {
        var item = await context.Set<TItem>().FindAsync(id);
        if (item == null)
        {
            return NotFound();
        }
        if (await OnBeforeDeleteAsync(item))
        {
            context.Set<TItem>().Remove(item);
            await context.SaveChangesAsync();
            return new NoContentResult();
        }
        else
            return Unauthorized();
    }

    protected virtual async Task<bool> OnBeforeDeleteAsync(TItem item)
    {
        await Task.Yield();
        return true;
    }


    private bool ValueExists(Guid key)
    {
        return context.Set<TItem>().Any(p => p.Id == key);
    }



}
