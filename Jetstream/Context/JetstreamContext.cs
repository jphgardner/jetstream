using Jetstream.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using NATS.Client;
using NATS.Client.JetStream;
using Polly;

namespace Jetstream.Context;

public interface IJetstreamContext
{
    IConnection GetConnection();
    IJetStream GetJetStream();
    IJetStreamManagement GetJetStreamManager();
}

public class JetstreamContext: IJetstreamContext
{
    private readonly JetstreamOptions _options;
    private IConnection _connection;
    private IJetStream _jetstream;
    private IJetStreamManagement _jetstreamManager;


    public JetstreamContext(IConfiguration configuration, IOptions<JetstreamOptions> options)
    {
        _options = options.Value;
        var connectionString = configuration.GetConnectionString(JetstreamOptions.ConfigurationKey);
        NATS.Client.Options opts = ConnectionFactory.GetDefaultOptions();
        opts.Servers = new []
        {
            connectionString
        };

        var natsSettings = options.Value;

        var retry = Policy.Handle<NATSConnectionException>()
            .WaitAndRetry(natsSettings.ReconnectAttempts,
                retry => TimeSpan.FromSeconds(natsSettings.ReconnectDurationInSeconds));

        retry.Execute(() =>
        {
            _connection = new ConnectionFactory().CreateConnection(opts);
            _jetstream = _connection.CreateJetStreamContext();
            _jetstreamManager = _connection.CreateJetStreamManagementContext();
        });
    }
    
    public IConnection GetConnection() => _connection;
    public IJetStream GetJetStream() => _jetstream;
    public IJetStreamManagement GetJetStreamManager() => _jetstreamManager;
}