using CsvHelper.Configuration.Attributes;

namespace StockChat.Stock.Models;

public class StockShareResponse
{
    [Index(0)]
    public string Symbol { get; set; }
    
    [Index(1)]
    public string Date { get; set; }
    
    [Index(2)]
    public string Time { get; set; }
    
    [Index(3)]
    public double Open { get; set; }
    
    [Index(4)]
    public double High { get; set; }
    
    [Index(5)]
    public double Low { get; set; }
    
    [Index(6)]
    public double Close { get; set; }
    
    [Index(7)]
    public long Volume { get; set; }
}