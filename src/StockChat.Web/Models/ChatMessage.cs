namespace StockChat.Web.Models;

public class ChatMessage
{
    public string? Id { get; set; }
    
    public string? ChatGroupId { get; set; }
    
    public string? SenderId { get; set; }

    public string? SenderName { get; set; }
    
    public string? Message { get; set; }
    
    public DateTime SentTime { get; set; }
}