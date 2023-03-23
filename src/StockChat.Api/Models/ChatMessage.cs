using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace StockChat.Api.Models;

public class ChatMessage
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    public string? ChatGroupId { get; set; }
    
    public string? SenderId { get; set; }

    public string? SenderName { get; set; }
    
    public string? Message { get; set; }
    
    public DateTime SentTime { get; set; }
}