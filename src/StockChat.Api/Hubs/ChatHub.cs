using Mediator;
using Microsoft.AspNetCore.SignalR;
using StockChat.Api.Handlers.Commands;
using StockChat.Api.Models;

namespace StockChat.Api.Hubs;

public class ChatHub : Hub
{
    private readonly ILogger<ChatHub> _logger;
    private readonly IMediator _mediator;
    
    public ChatHub(IMediator mediator, ILogger<ChatHub> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }
    
    public Task ConnectToLobby()
    {
        return Groups.AddToGroupAsync(Context.ConnectionId, "lobby");
    }
    
    public Task DisconnectFromLobby()
    {
        return Groups.RemoveFromGroupAsync(Context.ConnectionId, "lobby");
    }
    
    public Task SendConnectedToLobby(UserConnectedMessage user)
    {
        return Clients.Group("lobby").SendAsync("SendConnectedToLobby", user);
    }
    
    public Task AddStockChatGroup(string chatGroupId)
    {
        return Groups.AddToGroupAsync(Context.ConnectionId, chatGroupId);
    }

    public Task RemoveFromStockChatGroup(string chatGroupId)
    {
        return Groups.RemoveFromGroupAsync(Context.ConnectionId, chatGroupId);
    }
    
    public async Task SendMessageToStockChat(ChatMessage message)
    {
        if (message.ChatGroupId is null) return;
        if (message.Message is null) return;
        if (message.SenderId is null) return;
        if (message.SenderName is null) return;
        
        _logger.Log(LogLevel.Information, "Received message from chat group: {0}", message.ChatGroupId);
        _logger.Log(LogLevel.Information, "Received message from chat group: {0}", message.Message);
        
        if (message.Message.Contains("stock", StringComparison.InvariantCultureIgnoreCase))
        {
            var command = new DecodeStockCommand
            {
                ChatGroupId = message.ChatGroupId,
                StockCompany = message.Message.Replace("/stock=", "")
            };

            await _mediator.Send(command);
            return; 
        }
        
        var chatCommand = new RegisterChatMessageCommand
        {
            ChatGroupId = message.ChatGroupId,
            Message = message.Message,
            SenderId = message.SenderId,
            SenderName = message.SenderName,
            SentTime = message.SentTime
        };
        
        var id = await _mediator.Send(chatCommand);
        
        message.Id = id;
        
        await Clients.Group(message.ChatGroupId).SendAsync("SendStockChatMessage", message);

        
    }
}