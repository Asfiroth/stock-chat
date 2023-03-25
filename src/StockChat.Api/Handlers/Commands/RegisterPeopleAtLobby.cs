using Mediator;
using StockChat.Api.Data;
using StockChat.Api.Models;

namespace StockChat.Api.Handlers.Commands;

public class RegisterPeopleAtLobbyCommand : ICommand
{
    public string UserId { get; set; }
    public string UserName { get; set; }
}

public class RegisterPeopleAtLobby : ICommandHandler<RegisterPeopleAtLobbyCommand>
{
    private readonly IRepository<UserConnectedMessage> _repository;
    private readonly ILogger<RegisterChatMessage> _logger;

    public RegisterPeopleAtLobby(IRepository<UserConnectedMessage> repository, ILogger<RegisterChatMessage> logger)
    {
        _repository = repository;
        _logger = logger;
    }
    
    public async ValueTask<Unit> Handle(RegisterPeopleAtLobbyCommand command, CancellationToken cancellationToken)
    {
        // for demo purposes we are registering on mongo db
        // on real world we would register on a cache like redis
            
        _logger.Log(LogLevel.Information, "Registering people at lobby {0}", command.UserId);
            
        var exists = await _repository.GetFiltered(x => x.UserId == command.UserId);
            
        if (exists.Any())
        {
            _logger.Log(LogLevel.Information, "User already registered at lobby {0}", command.UserId);
            return default;
        }
            
        var userConnectedMessage = new UserConnectedMessage
        {
            UserId = command.UserId,
            UserName = command.UserName
        };

        await _repository.Register(userConnectedMessage);

        return default;
    }
}