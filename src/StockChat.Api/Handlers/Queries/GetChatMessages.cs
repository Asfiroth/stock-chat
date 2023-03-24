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

    public GetChatMessages(IRepository<ChatMessage> repository)
    {
        _repository = repository;
    }

    public async ValueTask<List<ChatMessage>> Handle(GetChatMessagesQuery query, CancellationToken cancellationToken)
    {
        // get from mongo db
        var chatMessages = await _repository.GetFiltered(x => x.ChatGroupId == query.ChatGroupId);
        
        var top50 = chatMessages.OrderByDescending(x => x.SentTime).Take(50).ToList();
        
        return top50;
    }
}