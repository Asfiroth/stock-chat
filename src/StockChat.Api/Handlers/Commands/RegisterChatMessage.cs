using Mediator;
using StockChat.Api.Data;
using StockChat.Api.Models;

namespace StockChat.Api.Handlers.Commands;

public class RegisterChatMessageCommand : ICommand
{
    public string ChatGroupId { get; set; }
    
    public string Id { get; set; }
    
    public string SenderId { get; set; }
    public string Message { get; set; }
    
    public DateTime SentTime { get; set; }
}

public class RegisterChatMessage : ICommandHandler<RegisterChatMessageCommand>
{
    private readonly IRepository<ChatMessage> _repository;

    public RegisterChatMessage(IRepository<ChatMessage> repository)
    {
        _repository = repository;
    }
    
    public ValueTask<Unit> Handle(RegisterChatMessageCommand command, CancellationToken cancellationToken)
    {
        // register on mongo db

        var chatMessage = new ChatMessage
        {
            ChatGroupId = command.ChatGroupId,
            Id = command.Id,
            Message = command.Message,
            SenderId = command.SenderId,
            SentTime = command.SentTime
        };

        _repository.Register(chatMessage);
        
        return default;
    }
}