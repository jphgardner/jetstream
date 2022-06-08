using Jetstream.Abstractions;
using Jetstream.Consumers;
using Jetstream.Context;
using Jetstream.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NATS.Client;

namespace Jetstream;

public static class JetstreamExtensions
{
    public static void AddJetstream(this IServiceCollection services, IConfiguration configuration)
    {
        var section = configuration.GetSection(JetstreamOptions.ConfigurationKey);
        services.Configure<JetstreamOptions>(section);
        services.AddSingleton<IJetstreamContext, JetstreamContext>();
    }
    
    public static void AddConsumer<TConsumer>(this IServiceCollection services)
        where TConsumer : class, IConsumer, IHostedService
    {
        services.AddHostedService<TConsumer>();
    }
    
    public static T Decoded<T>(this MsgHandlerEventArgs e) where T: IEvent
    {
        return Utility.Decode<T>(e.Message.Data);
    }
}