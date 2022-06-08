namespace Jetstream.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public class ConsumerAttribute: Attribute
{
    public readonly string Stream;
    public readonly string Subject;
    public readonly bool AutoAck;
    public readonly string? Queue;
    
    public ConsumerAttribute(string stream, string subject, bool autoAck, string? queue = null)
    {
        Stream = stream;
        Subject = subject;
        AutoAck = autoAck;
        Queue = queue;
    }
}
