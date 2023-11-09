using System.Runtime.Intrinsics.X86;
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
  
  public async Task AddCarAsync(CarDto car)
  {
   
    await _carRepository.AddAsync(new ()
    {
      Model = car.Model,
      Mark = car.Mark,
      Year = car.YearOfProduction,
      Color = car.Color,
      Price = car.Price,
      //VinCode = car.VinCode,
    });
  }

  public async Task<List<CarDto>> GetAll()
  {
    var dbCars = await _carRepository.GetAllAsync();
    return dbCars.Select(x => new CarDto(x.Model, x.Mark, x.Color, x.Year, x.Price, x.VinCode))
      .ToList();
  }
}