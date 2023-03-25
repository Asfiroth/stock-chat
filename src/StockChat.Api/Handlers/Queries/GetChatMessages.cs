using Mediator;
using StockChat.Api.Data;
using StockChat.Api.Models;

namespace StockChat.Api.Handlers.Queries;

public class GetChatMessagesQuery : IQuery<List<ChatMessage>>
{
    public string ChatGroupId { get; set; }
}

public class GetChatMessages : IQueryHandler<GetChatMessagesQuery, List<ChatMessage>>
{
    private readonly IRepository<ChatMessage> _repository;
    private readonly ILogger<GetChatMessages> _logger;

    public GetChatMessages(IRepository<ChatMessage> repository, ILogger<GetChatMessages> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async ValueTask<List<ChatMessage>> Handle(GetChatMessagesQuery query, CancellationToken cancellationToken)
    {
        if(query is null)
            throw new ArgumentNullException(nameof(query), "Invalid query");
            
        if(string.IsNullOrWhiteSpace(query.ChatGroupId))
            throw new ArgumentNullException(nameof(query.ChatGroupId), "Invalid chat group id");
            
        _logger.Log(LogLevel.Information, "Getting chat messages for group {0}", query.ChatGroupId);
        // get from mongo db
        var chatMessages = await _repository.GetFiltered(x => x.ChatGroupId == query.ChatGroupId);
        
        var top50 = chatMessages.OrderBy(x => x.SentTime).Take(50).ToList();
        
        return top50;
    }
}