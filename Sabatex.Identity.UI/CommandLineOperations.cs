using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sabatex.Identity.UI;

public interface ICommandLineOperations
{
    /// <summary>
    /// Run migrate  database
    /// </summary>
    /// <returns></returns>
    Task MigrateAsync();
    /// <summary>
    /// Garand user admin role
    /// </summary>
    /// <param name="userName"></param>
    /// <returns></returns>
    Task GrandUserAdminRoleAsync(string userName);
    Task InitialRolesAsync();
    Task InitialDemoDataAsync();

} 



public abstract class CommandLineOperations<DbContext>:ICommandLineOperations  where DbContext : IdentityDbContext
{
    readonly RoleManager<IdentityRole> roleManager;
    readonly UserManager<ApplicationUser> userManager;
    readonly DbContext dbContext;

    public CommandLineOperations(IServiceProvider serviceProvider)
    {
        roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        dbContext = serviceProvider.GetRequiredService<DbContext>();
    }
    public virtual async Task MigrateAsync()
    {
        await dbContext.Database.MigrateAsync();
    }

    public abstract Task InitialRolesAsync();
    

    public async Task GrandUserAdminRoleAsync(string userName)
    {
        await userManager.GrandUserAdminRoleAsync(userName);
    }

    public abstract Task InitialDemoDataAsync();
 
}
