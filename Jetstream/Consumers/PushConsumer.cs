using System.Reflection;
using Jetstream.Abstractions;
using Jetstream.Attributes;
using Jetstream.Context;
using Microsoft.Extensions.Hosting;
using NATS.Client;
using NATS.Client.JetStream;

namespace Jetstream.Consumers;

public abstract class PushConsumer<T> : BackgroundService, IConsumer
    where T : IEvent
{
    private readonly IJetstreamContext _jetstreamContext;
    private readonly IJetStreamManagement _jetstreamManager;
    private readonly ConsumerAttribute _attribute;
    private IJetStreamPushAsyncSubscription _subscription;
    
    protected StreamConfiguration.StreamConfigurationBuilder StreamConfig;
    protected ConsumerConfiguration.ConsumerConfigurationBuilder ConsumerConfig;

    protected StreamInfo StreamInfo { get; set; }
    protected ConsumerInfo ConsumerInfo { get; set; }

    public PushConsumer(IJetstreamContext jetstreamContext)
    {
        _jetstreamContext = jetstreamContext;
        _jetstreamManager = _jetstreamContext.GetJetStreamManager();

        _attribute = GetType().GetCustomAttribute<ConsumerAttribute>()!;
        
        StreamConfig = StreamConfiguration.Builder();
        StreamConfig.WithName(_attribute.Stream);
        StreamConfig.WithSubjects(_attribute.Subject);

        ConsumerConfig = ConsumerConfiguration.Builder();
        ConsumerConfig.WithDurable(GetType().Name);
    }

    public abstract Task ConsumeAsync(IPushConsumeContext<T> context);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        ConfigureStream();
        ConfigureConsumer();
        
        // Ensure stream exists
        await EnsureExistence();

        // Subscribe to stream
        await Subscribe();
    }

    private async Task Subscribe()
    {
        var jetstream = _jetstreamContext.GetJetStream();
        if (_attribute.Queue == null)
        {
            _subscription = jetstream.PushSubscribeAsync(_attribute.Subject, SubscriptionHandler, _attribute.AutoAck);
        }
        else
        {
            _subscription = jetstream.PushSubscribeAsync(_attribute.Subject, _attribute.Queue, SubscriptionHandler,
                _attribute.AutoAck);
        }
    }

    protected abstract void ConfigureStream();

    protected abstract void ConfigureConsumer();

    private async Task EnsureExistence()
    {
        var streamNames = _jetstreamManager.GetStreamNames();
        if (!streamNames.Contains(_attribute.Stream))
        {
            StreamInfo = _jetstreamManager.AddStream(StreamConfig.Build());
        }
        else
        {
            StreamInfo = _jetstreamManager.GetStreamInfo(_attribute.Stream);
            if (!StreamInfo.Config.Equals(StreamConfig.Build()))
            {
                StreamInfo = _jetstreamManager.UpdateStream(StreamConfig.Build());
            }
        }

        ConsumerInfo = _jetstreamManager.AddOrUpdateConsumer(_attribute.Stream, ConsumerConfig.Build());
        
        /*// check consumer exists
        var consumerNames = _jetstreamManager.GetConsumerNames(_attribute.Stream);
        if (!consumerNames.Contains(GetType().Name))
        {
            
        }
        else
        {
            ConsumerInfo = _jetstreamManager.GetConsumerInfo(_attribute.Stream, GetType().Name);
            if (!ConsumerInfo.ConsumerConfiguration.Equals(ConsumerConfig.Build()))
            {
                
            }
        }*/
    }

    private async void SubscriptionHandler(object? sender, MsgHandlerEventArgs message)
    {
        await ConsumeAsync(new PushConsumeContext<T>(message, _subscription));
        _jetstreamContext.GetConnection().Flush();
    }
}