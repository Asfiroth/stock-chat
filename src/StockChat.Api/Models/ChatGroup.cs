using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace StockChat.Api.Models;

// simple version of how a chatgroup should look like
public class ChatGroup : IEntity
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }
    
    public List<string> Members { get; set; }
}