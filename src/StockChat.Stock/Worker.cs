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
    
    private IModel _receiveChannel;
    private IModel _responseChannel;
    private IConnection _connection;
    private string _consumerTag;
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
        _receiveChannel = _connection.CreateModel();
        _responseChannel = _connection.CreateModel();
    }

    public override Task StartAsync(CancellationToken cancellationToken)
    {
        ConfigureRabbitMq();
        _logger.Log(LogLevel.Information, "Stock Chat Worker started");
        return base.StartAsync(cancellationToken);
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        //for demo purposes, we will create the exchange and queue here
        //in a real world scenario, we would create the exchange and queue in the infrastructure
        _receiveChannel.ExchangeDeclare(_options.Value.DecodeExchangeName, ExchangeType.Direct);
        _receiveChannel.QueueDeclare(_options.Value.DecodeQueueName, false, false, false);
        _receiveChannel.QueueBind(_options.Value.DecodeQueueName, _options.Value.DecodeExchangeName, _options.Value.DecodeRoutingKey);
        
        _responseChannel.ExchangeDeclare(_options.Value.ResponseExchangeName, ExchangeType.Direct);
        _responseChannel.QueueDeclare(_options.Value.ResponseQueueName, false, false, false);
        _responseChannel.QueueBind(_options.Value.ResponseQueueName, _options.Value.ResponseExchangeName, _options.Value.ResponseRoutingKey);
        
        _logger.Log(LogLevel.Information, "Stock Chat Worker is now listening for messages");
        
        _receiveChannel.BasicQos(0, 1, false);
        var consumer = new EventingBasicConsumer(_receiveChannel);
        consumer.Received += OnStockDecodingRequestReceived;
        
        _consumerTag = _receiveChannel.BasicConsume(_options.Value.DecodeQueueName, false, consumer);
        
        return Task.CompletedTask;
    }

    private async void OnStockDecodingRequestReceived(object? sender, BasicDeliverEventArgs args)
    {
        var content = Encoding.UTF8.GetString(args.Body.ToArray());
        
        _logger.Log(LogLevel.Information, "Received message to decode stock company: {0}", content);
        
        var message = JsonConvert.DeserializeObject<StockMessage>(content);
        
        if (message == null)
            throw new Exception("Invalid message received");
        
        // do some work
        var stockPrice = await _stockValueCheckService.CheckStock(message.StockCompany);
            
        // send response
        var response = new StockResponse
        {
            ChatGroupId = message.ChatGroupId,
            Message = $"{message.StockCompany.ToUpper()} is {stockPrice:C} per share"
        };
            
        var body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(response));
        _responseChannel.BasicPublish(_options.Value.ResponseExchangeName, _options.Value.ResponseRoutingKey, null, body);
            
        _receiveChannel.BasicAck(args.DeliveryTag, false);
    }

    public override Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.Log(LogLevel.Information, "Stock Chat Worker stopped");
        
        _receiveChannel.BasicCancel(_consumerTag);
        
        _receiveChannel.Close();
        _responseChannel.Close();
        _connection.Close();
        
        _receiveChannel.Dispose();
        _responseChannel.Dispose();
        _connection.Dispose();
        
        return base.StopAsync(cancellationToken);
    }
}