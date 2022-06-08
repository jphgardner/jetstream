using Jetstream.Abstractions;
using NATS.Client.JetStream;

namespace Jetstream.Context;

public interface IPullConsumeContext<TEvent>
    where TEvent: IEvent
{
    IJetStreamPullSubscription Subscription { get; set; }
}

public class PullConsumeContext<TEvent> : IPullConsumeContext<TEvent>
    where TEvent: IEvent
{
    public IJetStreamPullSubscription Subscription { get; set; }

    public PullConsumeContext(IJetStreamPullSubscription subscription)
    {
        Subscription = subscription;
    }

}