using SDPLabs.BusinessLogic;
using SDPLabs.DataAccess;
using SDPLabs.DataAccess.Interfaces;
using SDPLabs.DataAccess.Repositories;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using System.Threading;

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
app.MapPost("/car", (CarService service, CarRequestDto car)
  => service.AddCarAsync(new(car.Model, car.Mark, car.Color, car.YearOfProduction, car.Price, car.VinCode)));
app.MapGet("/cars", (CarService service) => service.GetAll());


app.Run();

public record CarRequestDto(string Model, string Mark, string Color, int YearOfProduction, int Price, string VinCode);


public class RabbitMqListenerService : IHostedService
{
  private IConnection _connection;
  private IModel _channel;
  public Task StartAsync(CancellationToken cancellationToken)
  {
    var factory = new ConnectionFactory()
    {
      HostName = "localhost",
      UserName = "user",
      Password = "password"
    };// Configure as needed
    _connection = factory.CreateConnection();
    _channel = _connection.CreateModel();

    _channel.QueueDeclare(queue: "yourQueueName", durable: false, exclusive: false, autoDelete: false, arguments: null);

    var consumer = new EventingBasicConsumer(_channel);
    consumer.Received += (model, ea) =>
    {
      var body = ea.Body.ToArray();
      var message = Encoding.UTF8.GetString(body);
      // Process the message here
      Console.WriteLine("Received message: " + message);
    };

    _channel.BasicConsume(queue: "yourQueueName", autoAck: true, consumer: consumer);

    return Task.CompletedTask;
  }
  public Task StopAsync(CancellationToken cancellationToken)
  {
    _channel?.Close();
    _connection?.Close();
    return Task.CompletedTask;
  }
}