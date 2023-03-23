using System.Globalization;
using System.Text;
using CsvHelper;
using CsvHelper.Configuration;
using StockChat.Stock.Models;

namespace StockChat.Stock.Services;

public class StockValueCheckService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<StockValueCheckService> _logger;
    
    public StockValueCheckService(ILogger<StockValueCheckService> logger, IHttpClientFactory httpClientFactory)
    {
        _logger = logger;
        _httpClientFactory = httpClientFactory;
    }
    
    public async Task<double> CheckStock(string stockCompany)
    {
        using var client = _httpClientFactory.CreateClient("stockApi");
        
        var response = await client.GetAsync($"q/l/?s={stockCompany}&f=sd2t2ohlcv&h&e=csv");
        if (!response.IsSuccessStatusCode) return 0;
        
        var csvStream = await response.Content.ReadAsStreamAsync();

        _logger.Log(LogLevel.Information, "Received csv stream from stock api");
        
        _logger.Log(LogLevel.Information, $"# of bytes in stream: {csvStream.Length}");
        
        var configuration = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            Delimiter = ",",
            Comment = '#',
            HasHeaderRecord = false
        };
        
        var stockShares = new List<StockShareResponse>();
        
        using (var reader = new StreamReader(csvStream))
        using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
        {
            csv.Context.RegisterClassMap<CsvStockShareResponseMapping>();
            stockShares = csv.GetRecords<StockShareResponse>().ToList();
        }
        
        _logger.Log(LogLevel.Information, "Parsed csv stream from stock api");
        
        if (!stockShares.Any()) return 0;
        
        var stockShare = stockShares.FirstOrDefault();
        
        if (stockShare == null) return 0;
        
        _logger.LogInformation($"Stock value for {stockCompany} is {stockShare.Close}");
        
        return stockShare.Close;
    }
}

// let's setup the mapping for the csv file here cause it's only used in this service
public class CsvStockShareResponseMapping : ClassMap<StockShareResponse>
{
    public CsvStockShareResponseMapping()
    {
        Map(x => x.Symbol).Index(0);
        Map(x => x.Date).Index(1);
        Map( x => x.Time).Index(2);
        Map( x => x.Open).Index(3);
        Map( x => x.High).Index(4);
        Map(x => x.Low).Index(5);
        Map( x => x.Close).Index(6);
        Map( x => x.Volume).Index(7);
    }
}