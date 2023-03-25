using Mediator;
using StockChat.Api.Data;
using StockChat.Api.Models;

namespace StockChat.Api.Handlers.Queries;

public class GetPeopleAtLobbyQuery : IQuery<List<UserConnectedMessage>>
{
    public static GetPeopleAtLobbyQuery Instance => new();
}

public class GetPeopleAtLobby : IQueryHandler<GetPeopleAtLobbyQuery, List<UserConnectedMessage>>
{
    private readonly IRepository<UserConnectedMessage> _repository;
    private readonly ILogger<GetPeopleAtLobby> _logger;

    public GetPeopleAtLobby(IRepository<UserConnectedMessage> repository, ILogger<GetPeopleAtLobby> logger)
    {
        _repository = repository;
        _logger = logger;
    }
    
    public async ValueTask<List<UserConnectedMessage>> Handle(GetPeopleAtLobbyQuery query, CancellationToken cancellationToken)
    {
        if (query is null)
            throw new ArgumentNullException(nameof(query), "Invalid query");
        
        _logger.Log(LogLevel.Information, "Getting people at lobby");
            
        // for demo purposes we are getting from mongo db
        // on real world we would register on a cache like redis
            
        var people = await _repository.GetAll();
        return people;
    }
}