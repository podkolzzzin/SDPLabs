using System;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
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
// Change the lifetime to Scoped for RabbitMqListenerService
builder.Services.AddScoped<RabbitMqListenerService>();

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
    private readonly CarService _carService;

    public RabbitMqListenerService(CarService carService)
    {
        _carService = carService;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        var factory = new ConnectionFactory()
        {
            HostName = "your_rabbitmq_host", // Replace with your RabbitMQ server host
            UserName = "your_username",      // Replace with your RabbitMQ username
            Password = "your_password"       // Replace with your RabbitMQ password
        };

        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();

        _channel.QueueDeclare(queue: "car_events_queue",
            durable: false,
            exclusive: false,
            autoDelete: false,
            arguments: null);

        var consumer = new EventingBasicConsumer(_channel);
        consumer.Received += async (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);

            try
            {
                
                var carMovedEvent = JsonSerializer.Deserialize<CarMovedEvent>(message);
                Console.WriteLine($"Car ID: {carMovedEvent?.Id}, Distance moved: {carMovedEvent?.Distance}");

                var createMileageDto =
                    new CreateMileageDto(carMovedEvent.Id, (int) carMovedEvent.Distance, DateTime.UtcNow);

                await _carService.AddMileageAsync(createMileageDto);
            }
            catch (JsonException ex)
            {
                Console.WriteLine($"Error deserializing message: {ex.Message}");
            }
        };

        _channel.BasicConsume(queue: "car_events_queue",
            autoAck: true,
            consumer: consumer);

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _channel?.Close();
        _connection?.Close();
        return Task.CompletedTask;
    }
}
