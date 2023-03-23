using System.Text;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using StockChat.Api.Hubs;
using StockChat.Api.Models;

namespace StockChat.Api.Services;

public class RabbitListenerService
{
    private IModel _channel;
    private IConnection _connection;
    private string _consumerTag;
    private readonly IOptions<RabbitOptions> _options;
    private readonly IHubContext<ChatHub> _hubContext;
    private readonly ILogger<RabbitListenerService> _logger;

    public RabbitListenerService(IOptions<RabbitOptions> options, IHubContext<ChatHub> hubContext, ILogger<RabbitListenerService> logger)
    {
        _options = options;
        _hubContext = hubContext;
        _logger = logger;
    }
    
    public void Register()
    {
        var connectionFactory = new ConnectionFactory();
        connectionFactory.Uri = new Uri(_options.Value.Connection);
        connectionFactory.ClientProvidedName = "StockChat.Api.ResponseListener";
        
        _connection = connectionFactory.CreateConnection();
        _channel = _connection.CreateModel();
        
        _channel.ExchangeDeclare(_options.Value.ResponseExchangeName, ExchangeType.Direct);
        _channel.QueueDeclare(_options.Value.ResponseQueueName, false, false, false);
        _channel.QueueBind(_options.Value.ResponseQueueName, _options.Value.ResponseExchangeName, _options.Value.ResponseRoutingKey);
        
        _channel.BasicQos(0, 1, false);
        var consumer = new EventingBasicConsumer(_channel);
        consumer.Received += OnStockDecodingResponseReceived;
        
        _consumerTag = _channel.BasicConsume(_options.Value.ResponseQueueName, false, consumer);
        
        _logger.Log(LogLevel.Information, "Stock Chat API is now listening for messages");
    }

    private void OnStockDecodingResponseReceived(object? sender, BasicDeliverEventArgs e)
    {
        var message = Encoding.UTF8.GetString(e.Body.ToArray());
        var response = JsonConvert.DeserializeObject<StockResponse>(message);
        
        _logger.Log(LogLevel.Information, $"Received stock response: {message}");
        
        if (response == null) return;
        
        var chatMessage = new ChatMessage
        {
            Id = Guid.NewGuid().ToString("D"),
            ChatGroupId = response.ChatGroupId,
            Message = response.Message,
            SenderName = "StockBot",
            SentTime = DateTime.Now
        };
        
        _hubContext.Clients.Group(response.ChatGroupId).SendAsync("SendStockChatMessage", chatMessage);
        _channel.BasicAck(e.DeliveryTag, false);
    }

    public void Deregister()
    {
        _channel.BasicCancel(_consumerTag);
        
        _channel.Close();
        _connection.Close();
        
        _channel.Dispose();
        _connection.Dispose();
    }
}