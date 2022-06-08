using Jetstream.Abstractions;
using NATS.Client;
using NATS.Client.JetStream;

namespace Jetstream.Context;

public interface IPushConsumeContext<TEvent>
    where TEvent : IEvent
{
    IJetStreamPushAsyncSubscription Subscription { get; }

    Msg Message { get; }
    TEvent? Event { get; }
}

public class PushConsumeContext<TEvent> : IPushConsumeContext<TEvent>
    where TEvent : IEvent
{
    public IJetStreamPushAsyncSubscription Subscription { get; }
    public Msg Message { get; }
    public TEvent? Event { get; }

    public PushConsumeContext(MsgHandlerEventArgs message, IJetStreamPushAsyncSubscription subscription)
    {
        Subscription = subscription;
        Message = message.Message;
        Event = message.Decoded<TEvent>();
    }
}