using Mediator;
using Microsoft.AspNetCore.SignalR;
using StockChat.Api.Handlers.Commands;
using StockChat.Api.Models;

namespace StockChat.Api.Hubs;

public class ChatHub : Hub
{
    private readonly IMediator _mediator;
    
    public ChatHub(IMediator mediator)
    {
        _mediator = mediator;
    }
    
    public async Task AddStockChatGroup(string chatGroupId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, chatGroupId);
    }

    public async Task RemoveFromStockChatGroup(string chatGroupId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, chatGroupId);
    }
    
    public async Task SendMessageToStockChat(ChatMessage message)
    {
        if (message.ChatGroupId is null) return;
        if (message.Message is null) return;
        if (message.Id is null) return;
        if (message.SenderId is null) return;

        if (message.Message.Contains("stock", StringComparison.InvariantCultureIgnoreCase))
        {
            var command = new DecodeStockCommand
            {
                ChatGroupId = message.ChatGroupId,
                StockCompany = message.Message
            };

            await _mediator.Send(command);
            return; 
        }
        
        await Clients.Group(message.ChatGroupId).SendAsync("SendStockChatMessage", message);

        var chatCommand = new RegisterChatMessageCommand
        {
            ChatGroupId = message.ChatGroupId,
            Message = message.Message,
            Id = message.Id,
            SenderId = message.SenderId,
            SentTime = message.SentTime
        };
        
        await _mediator.Send(chatCommand);
    }
}