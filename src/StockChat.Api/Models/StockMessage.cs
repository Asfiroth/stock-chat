namespace StockChat.Api.Models;

public class StockMessage
{
    public string ChatGroupId { get; set; }
    public string StockCompany { get; set; }
}

public class StockResponse
{
    public string ChatGroupId { get; set; }

    public string Message { get; set; }
}