using System.Reflection;
using Jetstream.Abstractions;
using Jetstream.Attributes;
using Jetstream.Context;
using Microsoft.Extensions.Hosting;
using NATS.Client.JetStream;

namespace Jetstream.Consumers;

public abstract class PullConsumer<T> : BackgroundService, IConsumer
    where T : IEvent
{
    private readonly IJetstreamContext _jetstreamContext;
    private readonly IJetStreamManagement _jetstreamManager;
    private readonly ConsumerAttribute _attribute;
    private IJetStreamPullSubscription _subscription;

    protected PullSubscribeOptions.PullSubscribeOptionsSubscribeOptionsBuilder SubscribeOptions =
        PullSubscribeOptions.Builder();

    public PullConsumer(IJetstreamContext jetstreamContext)
    {
        _jetstreamContext = jetstreamContext;
        _jetstreamManager = _jetstreamContext.GetJetStreamManager();

        _attribute = GetType().GetCustomAttribute<ConsumerAttribute>()!;

        SubscribeOptions.WithStream(_attribute.Stream);
        SubscribeOptions.WithDurable(GetType().Name);
    }

    public abstract void SetSubscribeOptions();
    public abstract Task ConsumeAsync(IPullConsumeContext<T> context);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // Subscribe to stream
        await Subscribe();
    }

    private async Task Subscribe()
    {
        SetSubscribeOptions();
        _subscription = _jetstreamContext.GetJetStream().PullSubscribe(_attribute.Stream, SubscribeOptions.Build());

        await ConsumeAsync(new PullConsumeContext<T>(_subscription));
    }
}