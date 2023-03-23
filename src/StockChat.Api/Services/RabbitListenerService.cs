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
    private readonly IOptions<RabbitOptions> _options;
    private readonly IHubContext<ChatHub> _hubContext;

    public RabbitListenerService(IOptions<RabbitOptions> options, IHubContext<ChatHub> hubContext)
    {
        _options = options;
        _hubContext = hubContext;
    }
    
    public void Register()
    {
        var connectionFactory = new ConnectionFactory();
        connectionFactory.Uri = new Uri(_options.Value.Connection);
        connectionFactory.ClientProvidedName = "StockChat.Api.ResponseListener";
        
        _connection = connectionFactory.CreateConnection();
        _channel = _connection.CreateModel();
        
        _channel.ExchangeDeclare(_options.Value.ResponseExchangeName, ExchangeType.Direct, true);
        _channel.QueueDeclare(_options.Value.ResponseQueueName, true, false, false);
        _channel.QueueBind(_options.Value.ResponseQueueName, _options.Value.ResponseExchangeName, _options.Value.ResponseRoutingKey);
        
        _channel.BasicQos(0, 1, false);
        var consumer = new EventingBasicConsumer(_channel);
        consumer.Received += OnStockDecodingResponseReceived;
    }

    private void OnStockDecodingResponseReceived(object? sender, BasicDeliverEventArgs e)
    {
        var message = Encoding.UTF8.GetString(e.Body.ToArray());
        var response = JsonConvert.DeserializeObject<StockResponse>(message);
        
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
    }

    public void Deregister()
    {
        _channel.Close();
        _connection.Close();
        
        _channel.Dispose();
        _connection.Dispose();
    }
}