namespace Sabatex.RadzenBlazor.Models;
/// <summary>
/// Add properties to this class and update the server and client AuthenticationStateProviders
/// to expose more information about the authenticated user to the client.
/// </summary>
public class UserInfo
{
    public required string UserId { get; set; }
    public required string Email { get; set; }
    public required string[] Roles  { get; set; }
}
