namespace StockChat.Stock.Services;

public interface IStockValueCheckService
{
    Task<double> CheckStock(string stockCompany);
}