using System.Text;
using Mediator;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RabbitMQ.Client;
using StockChat.Api.Models;

namespace StockChat.Api.Handlers.Commands;

public class DecodeStockCommand : ICommand
{
    public string ChatGroupId { get; set; }
    public string StockCompany { get; set; }
}

public class DecodeStockMessage : ICommandHandler<DecodeStockCommand>
{
    private readonly ILogger<DecodeStockMessage> _logger;
    private readonly IOptions<RabbitOptions> _options;
    public DecodeStockMessage(IOptions<RabbitOptions> options, ILogger<DecodeStockMessage> logger)
    {
        _options = options;
        _logger = logger;
    }
    
    public ValueTask<Unit> Handle(DecodeStockCommand command, CancellationToken cancellationToken)
    {
        // send to rabbit mq to be processed
        var connectionFactory = new ConnectionFactory();
        connectionFactory.Uri = new Uri(_options.Value.Connection);
        connectionFactory.ClientProvidedName = "StockChat.Api.DecodeRequester";
        
        var connection = connectionFactory.CreateConnection();
        var channel = connection.CreateModel();
        
        //for demo purposes, we will create the exchange and queue here
        //in a real world scenario, we would create the exchange and queue in the infrastructure
        channel.ExchangeDeclare(_options.Value.DecodeExchangeName, ExchangeType.Direct);
        channel.QueueDeclare(_options.Value.DecodeQueueName, false, false, false);
        channel.QueueBind(_options.Value.DecodeQueueName, _options.Value.DecodeExchangeName, _options.Value.DecodeRoutingKey);
        
        _logger.Log(LogLevel.Information, "Sending message to decode stock company: {0}", command.StockCompany);
        
        var message = new StockMessage
        {
            ChatGroupId = command.ChatGroupId,
            StockCompany = command.StockCompany
        };
        
        var body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(message));
        channel.BasicPublish(_options.Value.DecodeExchangeName, _options.Value.DecodeRoutingKey, null, body);
        
        channel.Close();
        connection.Close();
        
        return default;
    }
}