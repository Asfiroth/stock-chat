using System.Security.Claims;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;

namespace StockChat.Web.Pages;

[Authorize]
public class ChatLobbyModel : PageModel
{
    
    public MessageUser MessageUser { get; set; }

    public void OnGet()
    {
        var currentUser = User;
        var currentUserID = currentUser.FindFirst(ClaimTypes.NameIdentifier).Value;
        var currentUserName = currentUser.FindFirst("name").Value;

        MessageUser = new MessageUser
        {
            UserId = currentUserID,
            Name = currentUserName
        };
        
        ViewData["ChatHub"] = "https://chat:6010";
    }
}

public class MessageUser
{
    public string UserId { get; set; }
    public string Name { get; set; }
}