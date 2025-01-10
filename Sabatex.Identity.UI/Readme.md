# Identity UI for Blazor

This is a BackEnd library for Blazor .

## Getting Started
	
	### Install the package
```bash
dotnet add package sabatex.Identity.UI
```
	### Implement interface ICommandLineOperation
```csharp
public class CommandLineOperations: ICommandLineOperations
{
    readonly RoleManager<IdentityRole> roleManager;
    readonly UserManager<ApplicationUser> userManager;
    readonly ApplicationDbContext dbContext;

    public CommandLineOperations(RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager, ApplicationDbContext dbContext)
    {
        this.roleManager = roleManager;
        this.userManager = userManager;
        this.dbContext = dbContext;
    }
    public async Task GrandUserAdminRoleAsync(string userName)
    {
        await InitialRolesAsync();
        await userManager.GrandUserAdminRoleAsync(userName);
    }

    public async Task InitialDemoDataAsync()
    {
        await InitialRolesAsync();
        // Initial Demo Data

    }
    public async Task InitialRolesAsync()
    {
       // await roleManager.GetOrCreateRoleAsync(UserRole.Administrator);
    }
    public async Task MigrateAsync()
    {
        await dbContext.Database.MigrateAsync();
        await InitialRolesAsync();
    }
}


```

    ### Add the following code to the Program.cs file
```csharp
builder.Services.AddScoped<ICommandLineOperations, CommandLineOperations>();

...

// end file
app.MapAdditionalIdentityEndpoints();
await app.RunAsync(args);

```