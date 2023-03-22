using Mediator;
using StockChat.Api.Models;

namespace StockChat.Api.Handlers.Commands;

public class DecodeStockCommand : ICommand
{
    public string ChatGroupId { get; set; }
    public string StockCompany { get; set; }
}

public class DecodeStockMessage : ICommandHandler<DecodeStockCommand>
{
    public ValueTask<Unit> Handle(DecodeStockCommand command, CancellationToken cancellationToken)
    {
        // send to rabbit mq to be processed
        return default;
    }
}