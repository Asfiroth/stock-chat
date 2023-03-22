using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace StockChat.Web.Pages;

[Authorize]
public class ChatLobbyModel : PageModel
{
    
    public void OnGet()
    {
        
    }
}