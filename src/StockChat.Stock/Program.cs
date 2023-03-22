using StockChat.Stock;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        //services.Configure<WorkerOptions>(services.confi.GetSection("WorkerOptions"));
        services.AddHostedService<Worker>();
    })
    .Build();

host.Run();