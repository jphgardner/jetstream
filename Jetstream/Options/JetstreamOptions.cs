namespace Jetstream.Options;

public class JetstreamOptions
{
    public static string ConfigurationKey = "Jetstream";
    
    public int ReconnectAttempts { get; set; }
    public int ReconnectDurationInSeconds { get; set; }
}