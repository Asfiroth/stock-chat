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
        try
        {
            var chatGroupQuery = new GetChatGroupByMembersQuery
            {
                FakeGroupId = id
            };
        
            var groupId = await _mediator.Send(chatGroupQuery);
        
            var query = new GetChatMessagesQuery
            {
                ChatGroupId = groupId
            };
        
            var messages = await _mediator.Send(query);
        
            return Ok(messages);
        }
        catch (Exception e)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
        }
    }
    
    [HttpGet("lobby")]
    public async Task<IActionResult> GetLobby()
    {
        try
        {
            var query = GetPeopleAtLobbyQuery.Instance;
        
            var people = await _mediator.Send(query);
        
            return Ok(people);
        }
        catch (Exception e)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
        }
    }
}