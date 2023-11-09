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
    var dbCars = await _carRepository.GetAllAsync();
    foreach (var i in dbCars)
    {
      if (i.VinCode == car.VinCode)
      {
        //implement update method
        return;
      }
    }
    await _carRepository.AddAsync(new ()
    {
      Model = car.Model,
      Mark = car.Mark,
      Year = car.YearOfProduction,
      VinCode = car.VinCode
    });
  }

  public async Task<List<CarDto>> GetAll()
  {
    var dbCars = await _carRepository.GetAllAsync();
    return dbCars.Select(x => new CarDto(x.Model, x.Mark, null!, x.Year, 0, "Hidden"))
      .ToList();
  }

}