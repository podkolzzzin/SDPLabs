using SDPLabs.DataAccess;
using SDPLabs.DataAccess.Interfaces;

namespace SDPLabs.BusinessLogic;

public record CarDto(string Model, string Mark, string Color, int YearOfProduction, int Price, string VinCode);

public class CarService
{
  private readonly ICarRepository _carRepository;
  public CarService(ICarRepository carRepository)
  {
    _carRepository = carRepository;
  }
  
  public async Task AddCarAsync(CarDto carDto)
  {
    var existingCar = await _carRepository.FindByVinAsync(carDto.VinCode);
    if (existingCar != null)
    {
      existingCar.Model = carDto.Model;
      existingCar.Mark = carDto.Mark;
      existingCar.Color = carDto.Color;
      existingCar.Year = carDto.YearOfProduction;
      existingCar.Price = carDto.Price;
      await _carRepository.UpdateAsync(existingCar);
    }
    else
    {
      await _carRepository.AddAsync(new Car
      {
        Model = carDto.Model,
        Mark = carDto.Mark,
        Color = carDto.Color,
        Year = carDto.YearOfProduction,
        Price = carDto.Price,
        VinCode = carDto.VinCode
      });
    }
  }

  public async Task<List<CarDto>> GetAll()
  {
    var dbCars = await _carRepository.GetAllAsync();
    return dbCars.Select(x => new CarDto(x.Model, x.Mark, x.Color, x.Year, x.Price, string.Empty))
      .ToList();
  }
}