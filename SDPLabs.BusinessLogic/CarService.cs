using System.Text;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using RabbitMQ.Client;
using SDPLabs.DataAccess;
using SDPLabs.DataAccess.Interfaces;
using SDPLabs.Common.Events;
namespace SDPLabs.BusinessLogic;

public record AccidentDto(string Description, int Damage);
public record CarDto(string Model, string Mark, string Color, int YearOfProduction, int Price);
public record CreateCarDto(string Model, string Mark, string Color, int YearOfProduction, int Price, string VinCode);
public record CarWithMileageDto(string Model, string Mark, string Color, int YearOfProduction, int Price, double TotalMileage);

public class CarService
{
  private readonly ICarRepository _carRepository;
  private readonly SDPLabsDbContext _context; // Add DbContext to interact with Mileage
  private readonly RabbitMQConnectionManager _rabbitMQManager;


  // private readonly IModel _channel;

  public CarService(ICarRepository carRepository, SDPLabsDbContext context, RabbitMQConnectionManager rabbitMQManager)
  {
    _carRepository = carRepository;
    _context = context;
    _rabbitMQManager = rabbitMQManager;
  }
  
  public async Task AddCarAsync(CreateCarDto createCar)
  {
    var existing = await _carRepository.FindByVinCodeAsync(createCar.VinCode);
    if (existing != null)
    {
      existing.Color = createCar.Color;
      existing.Mark = createCar.Mark;
      existing.Year = createCar.YearOfProduction;
      existing.Price = createCar.Price;
      existing.Model = createCar.Model;
      await _carRepository.UpdateAsync(existing);
    }
    else
    {
      await _carRepository.AddAsync(new()
      {
        Model = createCar.Model,
        Mark = createCar.Mark,
        Year = createCar.YearOfProduction,
        Color = createCar.Color,
        Price = createCar.Price,
        VinCode = createCar.VinCode,
      });
    }
  }

  public async Task<List<CarDto>> GetAll()
  {
    
    // N + 1 Problem
    var dbCars = await _carRepository.GetAllAsync();
    return dbCars
      .Select(x => new CarDto(x.Model, x.Mark, x.Color, x.Year, x.Price))
      .ToList();
  }
  public async Task<List<CarWithMileageDto>> GetAllWithMileage()
  {
    var carsWithMileage = await _carRepository.GetAllWithMileageAsync();
    return carsWithMileage.Select(x => new CarWithMileageDto(
      x.Car.Model, 
      x.Car.Mark, 
      x.Car.Color, 
      x.Car.Year, 
      x.Car.Price,
      x.TotalMileage
    )).ToList();
  }
  public async Task AddMileageAsync(long carId, double distance)
  {
    var mileage = new Mileage
    {
      CarId = carId,
      Distance = distance,
      Date = DateTime.UtcNow // Assuming you want to record the current time
    };

    await _context.Mileages.AddAsync(mileage);
    await _context.SaveChangesAsync();
    
    var totalMileage = await CalculateTotalMileageAsync(carId);
    await CheckAndPublishMileageEvents(carId, totalMileage);
  }
  
  private async Task<double> CalculateTotalMileageAsync(long carId)
  {
    return await _context.Mileages
      .Where(m => m.CarId == carId)
      .SumAsync(m => m.Distance);
  }

  private async Task CheckAndPublishMileageEvents(long carId, double totalMileage)
  {
    if (totalMileage > 200000)
    {
      // Publish CarReceivedCriticalMileage event
      PublishMileageEvent(new CarReceivedCriticalMileage(carId, totalMileage));
    }
    else if (totalMileage > 100000)
    {
      // Publish CarReceivedDangerousMileage event
      PublishMileageEvent(new CarReceivedDangerousMileage(carId, totalMileage));
    }
  }
  
  
  private void PublishMileageEvent(object mileageEvent)
  {
    var eventName = mileageEvent.GetType().Name;
    var message = JsonSerializer.Serialize(mileageEvent);
    var body = Encoding.UTF8.GetBytes(message);
    
    var channel = _rabbitMQManager.GetChannel();

    channel.BasicPublish(
      exchange: nameof(CarMovedEvent), // Ensure this exchange is declared in your RabbitMQ setup
      routingKey: eventName,
      basicProperties: null,
      body: body
    );

    Console.WriteLine($"Published {eventName}: {message}");
  }

}