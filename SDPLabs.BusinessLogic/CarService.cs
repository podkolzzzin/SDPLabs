using SDPLabs.DataAccess.Interfaces;

namespace SDPLabs.BusinessLogic;

public record CarDto(string Model, string Mark, string Color, int YearOfProduction, int Price);
public record CreateCarDto(string Model, string Mark, string Color, int YearOfProduction, int Price, string VinCode);

public class CarService
{
  private readonly ICarRepository _carRepository;
  public CarService(ICarRepository carRepository)
  {
    _carRepository = carRepository;
  }

  public async Task AddCarAsync(CreateCarDto createCar)
  {
    var list = await _carRepository.GetAllAsync();
    var existing = list.FirstOrDefault(x => x.VinCode == createCar.VinCode);
    if (existing != null)
    {
      existing.Color = createCar.Color;
      existing.Model = createCar.Model;
      existing.Mark = createCar.Mark;
      existing.Year = createCar.YearOfProduction;
      existing.Price = createCar.Price;
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
        //VinCode = car.VinCode,
      });
    }
  }

  public async Task<List<CarDto>> GetAll()
  {
    var dbCars = await _carRepository.GetAllAsync();
    return dbCars.Select(x => new CarDto(x.Model, x.Mark, x.Color, x.Year, x.Price))
      .ToList();
  }
}