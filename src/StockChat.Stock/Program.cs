using System.Net.Http.Headers;
using StockChat.Stock;
using StockChat.Stock.Models;
using StockChat.Stock.Services;

var builder = Host.CreateDefaultBuilder(args);

builder.ConfigureServices((hostContext, services) =>
{
    services.AddHttpClient( "stockApi", client =>
    {
        var apiRoute = hostContext.Configuration["StockApi"];
        client.BaseAddress = new Uri(apiRoute ?? string.Empty);
        client.DefaultRequestHeaders.Clear();
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("text/csv"));
    });
    services.AddScoped<IStockValueCheckService, StockValueCheckService>();
    services.AddHostedService<Worker>();
    services.Configure<RabbitOptions>(hostContext.Configuration.GetSection("RabbitOptions"));
});

var host = builder.Build();

host.Run();