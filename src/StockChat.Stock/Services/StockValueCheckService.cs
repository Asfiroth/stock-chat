using System.Text;
using StockChat.Stock.Models;
using TinyCsvParser;
using TinyCsvParser.Mapping;
using TinyCsvParser.TypeConverter;

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
        
        var csvParserOptions = new CsvParserOptions(true, ',');
        var csvParser = new CsvParser<StockShareResponse>(csvParserOptions, new CsvStockShareResponseMapping());

        var stockShares = csvParser.ReadFromStream(csvStream, Encoding.UTF8)
            .Select(x => x.Result)
            .ToList();
        
        if (!stockShares.Any()) return 0;
        
        var stockShare = stockShares.FirstOrDefault();
        
        if (stockShare == null) return 0;
        
        _logger.LogInformation($"Stock value for {stockCompany} is {stockShare.Close}");
        
        return stockShare.Close;
    }
}

// let's setup the mapping for the csv file here cause it's only used in this service
public class CsvStockShareResponseMapping : CsvMapping<StockShareResponse>
{
    public CsvStockShareResponseMapping() : base()
    {
        MapProperty(0, x => x.Symbol);
        MapProperty(1, x => x.Date);
        MapProperty(2, x => x.Time);
        MapProperty(3, x => x.Open);
        MapProperty(4, x => x.High);
        MapProperty(5, x => x.Low);
        MapProperty(5, x => x.Close);
        MapProperty(5, x => x.Volume);
    }
}