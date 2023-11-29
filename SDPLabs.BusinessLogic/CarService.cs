using SDPLabs.DataAccess;
using SDPLabs.DataAccess.Interfaces;

namespace SDPLabs.BusinessLogic;

public record AccidentDto(string Description, int Damage);
public record CarDto(string Model, string Mark, string Color, int YearOfProduction, int Price);
public record CreateCarDto(string Model, string Mark, string Color, int YearOfProduction, int Price, string VinCode);
public record CarWithMileageDto(string Model, string Mark, string Color, int YearOfProduction, int Price, double TotalMileage);

public class CarService
{
  private readonly ICarRepository _carRepository;
  private readonly SDPLabsDbContext _context; // Add DbContext to interact with Mileage
  public CarService(ICarRepository carRepository, SDPLabsDbContext context)
  {
    _carRepository = carRepository;
    _context = context;
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
  }
}