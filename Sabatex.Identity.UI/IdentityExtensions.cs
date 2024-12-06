using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sabatex.Identity.UI;

/// <summary>
/// Extended Identity
/// </summary>
public static class IdentityExtensions
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="configuration"></param>
    /// <returns></returns>
    public static AuthenticationBuilder AddIsConfiguredGoogle(this AuthenticationBuilder builder,IConfiguration configuration)
    {
        var clientId = configuration["Authentication:Google:ClientId"];
        var clientSecret = configuration["Authentication:Google:ClientSecret"];
        if (clientId != null && clientSecret != null)
        {
            builder.Services.AddAuthentication().AddGoogle(options =>
            {
                options.ClientId = clientId;
                options.ClientSecret = clientSecret;
            });
        }
        return builder;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="configuration"></param>
    /// <returns></returns>
    public static AuthenticationBuilder AddIsConfiguredMicrosoft(this AuthenticationBuilder builder, IConfiguration configuration)
    {
        var clientId = configuration["Authentication:Microsoft:ClientId"];
        var clientSecret = configuration["Authentication:Microsoft:ClientSecret"];
        if (clientId != null && clientSecret != null)
        {
            builder.Services.AddAuthentication().AddMicrosoftAccount(options =>
            {
                options.ClientId = clientId;
                options.ClientSecret = clientSecret;
            });
        }
        return builder;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="manager"></param>
    /// <param name="userFileConfig"></param>
    /// <returns></returns>
    public static ConfigurationManager AddUserConfiguration(this ConfigurationManager manager,string userFileConfig)
    {
        var confogFileName = $"/etc/sabatex/{userFileConfig}";
        if (File.Exists(confogFileName))
            manager.AddJsonFile(confogFileName,optional:true,reloadOnChange:false);
        return manager;
    }

}
