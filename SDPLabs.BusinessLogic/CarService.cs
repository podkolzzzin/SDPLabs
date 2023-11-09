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
    await _carRepository.AddAsync(new ()
    {
      Model = carDto.Model,
      Mark = carDto.Mark,
      Color = carDto.Color,
      Year = carDto.YearOfProduction,
      Price = carDto.Price,
      VinCode = carDto.VinCode
    });
  }

  public async Task<List<CarDto>> GetAll()
  {
    var dbCars = await _carRepository.GetAllAsync();
    return dbCars.Select(x => new CarDto(x.Model, x.Mark, x.Color, x.Year, x.Price, string.Empty))
      .ToList();
  }
}