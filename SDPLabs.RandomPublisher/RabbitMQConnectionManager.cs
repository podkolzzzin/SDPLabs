using RabbitMQ.Client;
using SDPLabs.Common.Events;

public class RabbitMQConnectionManager
{
    private readonly ConnectionFactory _factory;
    private IConnection _connection;
    private IModel _channel;

    public RabbitMQConnectionManager(string hostName, string userName, string password)
    {
        _factory = new ConnectionFactory()
        {
            HostName = hostName,
            UserName = userName,
            Password = password
        };

        InitializeConnection();
    }

    private void InitializeConnection()
    {
        _connection = _factory.CreateConnection();
        _channel = _connection.CreateModel();

        // Declare the exchange you need
        _channel.ExchangeDeclare(nameof(CarMovedEvent), ExchangeType.Fanout);
    }

    public IModel GetChannel()
    {
        return _channel;
    }

    // Implement IDisposable to properly dispose of the connection and channel
    public void Dispose()
    {
        _channel?.Dispose();
        _connection?.Dispose();
    }
}