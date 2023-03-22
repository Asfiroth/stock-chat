using Mediator;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StockChat.Api.Handlers.Queries;

namespace StockChat.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class ChatInputController : ControllerBase
{
    private readonly IMediator _mediator;
    
    public ChatInputController(IMediator mediator)
    {
        _mediator = mediator;
    }
    
    [HttpGet("{id}/messages")]
    public async Task<IActionResult> Get(string id)
    {
        var query = new GetChatMessagesQuery
        {
            ChatGroupId = id
        };
        
        var messages = await _mediator.Send(query);
        
        return Ok(messages);
    }
}