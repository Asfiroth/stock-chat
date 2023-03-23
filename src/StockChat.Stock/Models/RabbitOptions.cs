namespace StockChat.Stock.Models;

public class RabbitOptions
{
    public string Connection { get; set; }
    public string DecodeExchangeName { get; set; }
    public string DecodeQueueName { get; set; }
    public string DecodeRoutingKey { get; set; }
    public string ResponseExchangeName { get; set; }
    public string ResponseQueueName { get; set; }
    public string ResponseRoutingKey { get; set; }
}