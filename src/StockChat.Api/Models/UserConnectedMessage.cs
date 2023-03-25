using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace StockChat.Api.Models;

public class UserConnectedMessage : IEntity
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }
    
    public string UserId { get; set; }
    public string UserName { get; set; }
}