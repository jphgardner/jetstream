using Jetstream.Abstractions;

namespace Microservice.Events;

public class TestEvent: IEvent
{
    public string Name { get; set; }
}