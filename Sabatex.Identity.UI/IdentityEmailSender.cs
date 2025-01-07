using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;

using System.Net;
using System.Security.Authentication;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;


namespace Sabatex.Identity.UI;

// Remove the "else if (EmailSender is IdentityNoOpEmailSender)" block from RegisterConfirmation.razor after updating with a real implementation.
public sealed class IdentityEmailSender : IEmailSender<ApplicationUser>
{
    private readonly IConfiguration Configuration;

    public IdentityEmailSender(IConfiguration configuration)
    {
        Configuration = configuration;
    }
    async Task SendEmailAsync(string email, string subject, string message)
    {
        var MailServer = Configuration.GetSection("MailServer");
        var pass = MailServer.GetValue<string>("Pass");
        var login = MailServer.GetValue<string>("Login");
        var port = MailServer.GetValue<int>("Port");
        var host = MailServer.GetValue<string>("SMTPHost");

        var smtpClient = new SmtpClient()
        {
            Host = host, // set your SMTP server name here
            Port = port, // Port 
            EnableSsl = true,
            Credentials = new NetworkCredential(login, pass)
        };

        using (var mail = new MailMessage(login, email, subject, message))
        {
            mail.IsBodyHtml = true;
            await smtpClient.SendMailAsync(mail);
        }



    }
    public Task SendConfirmationLinkAsync(ApplicationUser user, string email, string confirmationLink) =>
        SendEmailAsync(email, "Confirm your email", $"Please confirm your account by <a href='{confirmationLink}'>clicking here</a>.");

    public Task SendPasswordResetLinkAsync(ApplicationUser user, string email, string resetLink) =>
        SendEmailAsync(email, "Reset your password", $"Please reset your password by <a href='{resetLink}'>clicking here</a>.");

    public Task SendPasswordResetCodeAsync(ApplicationUser user, string email, string resetCode) =>
        SendEmailAsync(email, "Reset your password", $"Please reset your password using the following code: {resetCode}");
}
