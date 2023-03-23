using Mediator;
using StockChat.Api.Data;
using StockChat.Api.Models;

namespace StockChat.Api.Handlers.Commands;

public class RegisterChatMessageCommand : ICommand<string>
{
    public string ChatGroupId { get; set; }
    public string SenderId { get; set; }
    public string SenderName { get; set; }
    public string Message { get; set; }
    
    public DateTime SentTime { get; set; }
}

public class RegisterChatMessage : ICommandHandler<RegisterChatMessageCommand, string>
{
    private readonly IRepository<ChatMessage> _repository;
    private readonly ILogger<RegisterChatMessage> _logger;

    public RegisterChatMessage(IRepository<ChatMessage> repository, ILogger<RegisterChatMessage> logger)
    {
        _repository = repository;
        _logger = logger;
    }
    
    public async ValueTask<string> Handle(RegisterChatMessageCommand command, CancellationToken cancellationToken)
    {
        // register on mongo db

        try
        {
            _logger.Log(LogLevel.Information, "Registering chat message {0}", command.Message);
            
            var chatMessage = new ChatMessage
            {
                ChatGroupId = command.ChatGroupId,
                Message = command.Message,
                SenderId = command.SenderId,
                SenderName = command.SenderName,
                SentTime = command.SentTime
            };

            var messageId = await _repository.Register(chatMessage);

            return messageId;
        }
        catch (Exception e)
        {
            _logger.Log(LogLevel.Error, e, "Error registering chat message");
            return string.Empty;
        }
    }
}