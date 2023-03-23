using System.Text;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using StockChat.Stock.Models;
using StockChat.Stock.Services;

namespace StockChat.Stock;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    
    private IModel _channel;
    private IConnection _connection;
    private readonly IOptions<RabbitOptions> _options;
    private readonly StockValueCheckService _stockValueCheckService;

    public Worker(ILogger<Worker> logger, IOptions<RabbitOptions> options, StockValueCheckService stockValueCheckService)
    {
        _logger = logger;
        _options = options;
        _stockValueCheckService = stockValueCheckService;
    }
    
    private void ConfigureRabbitMq()
    {
        var connectionFactory = new ConnectionFactory();
        connectionFactory.Uri = new Uri(_options.Value.Connection);
        connectionFactory.ClientProvidedName = "StockChat.Stock";
        
        _connection = connectionFactory.CreateConnection();
        _channel = _connection.CreateModel();
        
        //for demo purposes, we will create the exchange and queue here
        //in a real world scenario, we would create the exchange and queue in the infrastructure
        _channel.ExchangeDeclare(_options.Value.DecodeExchangeName, ExchangeType.Direct, true);
        _channel.QueueDeclare(_options.Value.DecodeQueueName, true, false, false);
        _channel.QueueBind(_options.Value.DecodeQueueName, _options.Value.DecodeExchangeName, _options.Value.DecodeRoutingKey);
        
        _channel.ExchangeDeclare(_options.Value.ResponseExchangeName, ExchangeType.Direct, true);
        _channel.QueueDeclare(_options.Value.ResponseQueueName, true, false, false);
        _channel.QueueBind(_options.Value.ResponseQueueName, _options.Value.ResponseExchangeName, _options.Value.ResponseRoutingKey);
    }

    public override Task StartAsync(CancellationToken cancellationToken)
    {
        ConfigureRabbitMq();
        return base.StartAsync(cancellationToken);
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _channel.BasicQos(0, 1, false);
        var consumer = new EventingBasicConsumer(_channel);
        consumer.Received += OnStockDecodingRequestReceived;

        return Task.CompletedTask;
    }

    private void OnStockDecodingRequestReceived(object? sender, BasicDeliverEventArgs args)
    {
        var content = Encoding.UTF8.GetString(args.Body.ToArray());
        var message = JsonConvert.DeserializeObject<StockMessage>(content);

        if (message == null)
            throw new Exception("Invalid message received");
        
        // do some work
        var stockPrice = _stockValueCheckService.CheckStock(message.StockCompany);
            
        // send response
        var response = new StockResponse
        {
            ChatGroupId = message.ChatGroupId,
            Message = $"{message.StockCompany.ToUpper()} is {stockPrice:C} per share"
        };
            
        var body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(response));
        _channel.BasicPublish(_options.Value.ResponseExchangeName, _options.Value.ResponseRoutingKey, null, body);
            
        _channel.BasicAck(args.DeliveryTag, false);
    }

    public override Task StopAsync(CancellationToken cancellationToken)
    {
        _channel.Close();
        _connection.Close();
        
        _channel.Dispose();
        _connection.Dispose();
        
        return base.StopAsync(cancellationToken);
    }
}