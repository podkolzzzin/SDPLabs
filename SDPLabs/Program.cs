using SDPLabs.BusinessLogic;
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
app.MapPost("/car", (CarService service, CarRequestDto car)
  => service.AddCarAsync(new(car.Model, car.Mark, car.Color, car.YearOfProduction, car.Price, car.VinCode)));
app.MapGet("/cars", (CarService service) => service.GetAll());


app.Run();

public record CarRequestDto(string Model, string Mark, string Color, int YearOfProduction, int Price, string VinCode);

public class RabbitMqListenerService : IHostedService
{
  public Task StartAsync(CancellationToken cancellationToken)
  {
    // Open Connection and Channel here
    // Start listening to events
    throw new NotImplementedException();
  }
  public Task StopAsync(CancellationToken cancellationToken)
  {
    // Close Connection and Channel here
    throw new NotImplementedException();
  }
}