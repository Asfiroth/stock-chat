using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using StockChat.Web.Models;

namespace StockChat.Web.Pages;

[Authorize]
public class ChatLobbyModel : PageModel
{
    private readonly IConfiguration _configuration;
    public ChatLobbyModel(IConfiguration configuration): base()
    {
        _configuration = configuration;
    }
    
    public MessageUser MessageUser { get; set; }

    public async Task OnGetAsync()
    {
        var currentUserID = User.FindFirst(ClaimTypes.NameIdentifier).Value;
        var currentUserName = User.FindFirst("name").Value;

        MessageUser = new MessageUser
        {
            UserId = currentUserID,
            Name = currentUserName
        };
        
        ViewData["ChatHub"] = _configuration["ChatApi"];
    }
    
    public async Task<IActionResult> OnGetChatMessages(string id)
    {
        var client = new HttpClient();
        var token = await HttpContext.GetTokenAsync("access_token");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        var response = await client.GetAsync($"{_configuration["ChatApi"]}/api/chatinput/{id}/messages");
        var content = await response.Content.ReadAsStringAsync();
        var messages = JsonConvert.DeserializeObject<List<ChatMessage>>(content);
        return new JsonResult(messages);
    }

    public async Task<IActionResult> OnGetPeopleAtLobby()
    {
        var client = new HttpClient();
        var token = await HttpContext.GetTokenAsync("access_token");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        var response = await client.GetAsync($"{_configuration["ChatApi"]}/api/chatinput/lobby");
        var content = await response.Content.ReadAsStringAsync();
        var users = JsonConvert.DeserializeObject<List<LobbyUser>>(content);
        
        var currentUserID = User.FindFirst(ClaimTypes.NameIdentifier).Value;
        
        var result = users?.Where(c => c.UserId != currentUserID).ToList() ?? new List<LobbyUser>();

        return new JsonResult(result);
    }
}

public class MessageUser
{
    public string UserId { get; set; }
    public string Name { get; set; }
}