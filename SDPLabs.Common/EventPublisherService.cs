using System.Text;
using System.Text.Json;
using RabbitMQ.Client;
using SDPLabs.Common.Interfaces;

namespace SDPLabs.Common;

public class EventPublisherService : IEventPublisherService
{
  public const string ExchangeName = nameof(EventPublisherService);
  private readonly IModel _channel;
  
  public EventPublisherService(string host, string user, string password)
  {
    var factory = new ConnectionFactory()
    {
      HostName = host, // Change to the appropriate RabbitMQ server address if needed
      UserName = user,
      Password = password,
    };
    
    var connection = factory.CreateConnection();
    _channel = connection.CreateModel();
    
    _channel.ExchangeDeclare(ExchangeName, ExchangeType.Fanout);
  }
  public void Publish<T>(T @event) where T : notnull
  {
    var properties = _channel.CreateBasicProperties();
    properties.Headers = new Dictionary<string, object>()
    {
      ["EventType"] = @event.GetType().FullName!
    };
    var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(@event));
    _channel.BasicPublish(
      ExchangeName, 
      routingKey: "", 
      basicProperties: properties,
      body: body);
  }
}