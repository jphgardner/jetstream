using Jetstream.Attributes;
using Jetstream.Consumers;
using Jetstream.Context;
using Microservice.Events;
using NATS.Client.JetStream;

namespace Microservice.Consumers;


[Consumer("Test", "Test.*", false)]
public class TestPushConsumer: PushConsumer<TestEvent>
{
    public TestPushConsumer(IJetstreamContext jetstreamContext) : base(jetstreamContext)
    {
    }
    
    protected override void ConfigureStream()
    {
    }

    protected override void ConfigureConsumer()
    {
        ConsumerConfig.WithDeliverPolicy(DeliverPolicy.New);
        ConsumerConfig.WithAckWait(6000);
        ConsumerConfig.WithAckPolicy(AckPolicy.Explicit);
    }

    public override async Task ConsumeAsync(IPushConsumeContext<TestEvent> context)
    {
        try
        {
            
            Console.WriteLine("TestConsumer");
            context.Message.AckSync(1000);
        }
        catch (Exception e)
        {
            Console.WriteLine("Exception: " + e.Message);
        }
    }
}