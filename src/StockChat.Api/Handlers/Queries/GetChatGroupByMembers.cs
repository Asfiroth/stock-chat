using Mediator;
using StockChat.Api.Models;

namespace StockChat.Api.Handlers.Queries;

public class GetChatGroupByMembersQuery : IQuery<string>
{
    public string FakeGroupId { get; set; }
}

public class GetCharGroupByMembers : IQueryHandler<GetChatGroupByMembersQuery, string>
{
    public ValueTask<string> Handle(GetChatGroupByMembersQuery query, CancellationToken cancellationToken)
    {
        // now we use this on memory, but in a real world scenario we would use cache like redis
        // or even a database
        
        if (query is null)
            throw new ArgumentNullException(nameof(query), "Invalid query");

        if (string.IsNullOrWhiteSpace(query.FakeGroupId))
            throw new ArgumentNullException(nameof(query.FakeGroupId), "Invalid chat group id");
        
        var members = query.FakeGroupId.Split('|').ToList();
        
        if (members.Count != 2) 
            throw new Exception("Invalid chat group id");
        
        members.Sort();
        
        var chatGroupId = string.Join("-", members);
        
        return ValueTask.FromResult(chatGroupId);
    }
}