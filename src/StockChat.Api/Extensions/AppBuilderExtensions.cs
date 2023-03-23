using StockChat.Api.Services;
namespace StockChat.Api.Extensions;

public static class AppBuilderExtensions
{
    private static RabbitListenerService _listener;
    
    public static IApplicationBuilder UseRabbitListener(this IApplicationBuilder app)
    {
        _listener = app.ApplicationServices.GetService<RabbitListenerService>();

        var lifetime = app.ApplicationServices.GetService<IHostApplicationLifetime>();

        if (lifetime == null) return app;
        
        lifetime.ApplicationStarted.Register(OnStarted);
        lifetime.ApplicationStopping.Register(OnStopping);

        return app;
    }

    private static void OnStarted()
    {
        _listener.Register();
    }

    private static void OnStopping()
    {
        _listener.Deregister();    
    }
}