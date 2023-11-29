using SDPLabs.DataAccess;
using SDPLabs.DataAccess.Interfaces;

namespace SDPLabs.BusinessLogic;

public record AccidentDto(string Description, int Damage);
public record CarDto(string Model, string Mark, string Color, int YearOfProduction, int Price, int Mileage, DateTime recordTime);
public record CreateCarDto(string Model, string Mark, string Color, int YearOfProduction, int Price, string VinCode);

public record MileageDto(long CarId, int Distance, DateTime DateRecorded);
public record CreateMileageDto(long CarId, int Distance, DateTime DateRecorded);


public class CarService
{
  private readonly ICarRepository _carRepository;
  private readonly IMileageRepository _mileageRepository;

  public CarService(ICarRepository carRepository, IMileageRepository mileageRepository)
  {
    _carRepository = carRepository;
    _mileageRepository = mileageRepository;
  }

  public async Task AddMileageAsync(CreateMileageDto createMileageDto)
  {
    var mileage = new Mileage
    {
      CarId = createMileageDto.CarId,
      Distance = createMileageDto.Distance,
      DateRecorded = createMileageDto.DateRecorded
    };
    await _mileageRepository.AddAsync(mileage);
  }

  public async Task<List<MileageDto>> GetMileageByCarIdAsync(long carId)
  {
    var mileages = await _mileageRepository.FindByCarIdAsync(carId);
    return mileages.Select(m => new MileageDto(m.CarId, m.Distance, m.DateRecorded)).ToList();
  }
  public CarService(ICarRepository carRepository)
  {
    _carRepository = carRepository;
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
    var dbCars = await _carRepository.GetAllAsync();
    var carDtos = new List<CarDto>();

    foreach (var dbCar in dbCars)
    {
      var totalMileage = 0;
      var mileages = await _mileageRepository.GetAllMileagesAsync();
      foreach (var tt in mileages)
      {
        if (tt.Car == dbCar)
        {
          totalMileage += tt.Distance;
        }
      }
      carDtos.Add(new CarDto(
        dbCar.Model,
        dbCar.Mark,
        dbCar.Color,
        dbCar.Year,
        dbCar.Price,
        totalMileage,
        DateTime.UtcNow
      ));
    }

    return carDtos;
  }
}