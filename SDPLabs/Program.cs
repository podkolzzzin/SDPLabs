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

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.MapGet("/", () => "Hello World!");
app.MapPost("/car", (CarService service, CarRequestDto car)
  => service.AddCarAsync(new(car.Model, car.Mark, car.Color, car.YearOfProduction, car.Price, car.Vincode)));
app.MapGet("/cars", (CarService service) => service.GetAll());

app.Run();

public record CarRequestDto(string Model, string Mark, string Color, int YearOfProduction, int Price, string Vincode);