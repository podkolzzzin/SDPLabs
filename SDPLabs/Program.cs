using System.Text;
using System.Text.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using SDPLabs.BusinessLogic;
using SDPLabs.Common.Events;
using SDPLabs.DataAccess;
using SDPLabs.DataAccess.Interfaces;
using SDPLabs.DataAccess.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<SDPLabsDbContext>();
builder.Services.AddScoped<CarService>();
builder.Services.AddScoped<ICarRepository, CarRepository>();
builder.Services.AddHostedService<RabbitMqListenerService>();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.MapGet("/", () => "Hello World!");
app.MapPost("/car", (CarService service , CarRequestDto car)
    => service.AddCarAsync(new(car.Model, car.Mark, car.Color, car.YearOfProduction, car.Price, car.VinCode)));
app.MapGet("/cars", (CarService service) => service.GetAll());


app.Run();


public record CarRequestDto(string Model, string Mark, string Color, int YearOfProduction, int Price, string VinCode);

public class RabbitMqListenerService : IHostedService
{
  private readonly IServiceProvider _serviceProvider;
  private IConnection? _connection;
  private IModel? _channel;
  private string _queueName;
  
  public RabbitMqListenerService(IServiceProvider serviceProvider)
  {
    _serviceProvider = serviceProvider;
  }

  
  public Task StartAsync(CancellationToken cancellationToken)
  {

    var factory = new ConnectionFactory()
    {
      HostName = "localhost",
      UserName = "user",
      Password = "password"
    };

    _connection = factory.CreateConnection();
    _channel = _connection.CreateModel();

    _channel.ExchangeDeclare(nameof(CarMovedEvent), ExchangeType.Fanout);
    _queueName = _channel.QueueDeclare().QueueName;
    _channel.QueueBind(queue: _queueName, exchange: nameof(CarMovedEvent), routingKey: "");

    var consumer = new EventingBasicConsumer(_channel);
    consumer.Received += async (model, ea) =>
    {
      var body = ea.Body.ToArray();
      var message = Encoding.UTF8.GetString(body);
      var carMovedEvent = JsonSerializer.Deserialize<CarMovedEvent>(message);
      using var scope = _serviceProvider.CreateScope();
      var carService = scope.ServiceProvider.GetRequiredService<CarService>();

      try
      {
        await carService.AddMileageAsync(new CreateMileageDto(carMovedEvent.Id, (int)carMovedEvent.Distance, DateTime.UtcNow));
        Console.WriteLine($"Mileage added for Car ID: {carMovedEvent.Id}");
      }
      catch (Exception ex)
      {
        Console.WriteLine($"Error adding mileage: {ex.Message}");
      }
      Console.WriteLine("Received: {0}", message);
    };

    _channel.BasicConsume(queue: _queueName, autoAck: true, consumer: consumer);
  
    return Task.CompletedTask;
  }

  public Task StopAsync(CancellationToken cancellationToken)
  {
    _channel?.Close();
    _connection?.Close();
    return Task.CompletedTask;
  }
}