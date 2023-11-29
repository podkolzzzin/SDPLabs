using Microsoft.EntityFrameworkCore;
using SDPLabs.DataAccess.Interfaces;

namespace SDPLabs.DataAccess.Repositories;

public class CarRepository : ICarRepository
{
  private readonly SDPLabsDbContext _context;
  public CarRepository(SDPLabsDbContext context)
  {
    _context = context;
  }
  
  public class CarWithTotalMileage
  {
    public Car Car { get; set; }
    public double TotalMileage { get; set; }
  }


  public async Task<List<Car>> GetAllAsync()
  {
    return await _context.Cars
      .ToListAsync();
  }
  
  public async Task<Car> AddAsync(Car car)
  {
    _context.Add(car);
    await _context.SaveChangesAsync();
    return car;
  }
  
  public async Task UpdateAsync(Car existing)
  {
    _context.Update(existing);
    await _context.SaveChangesAsync();
  }

  public async Task<Car?> FindByVinCodeAsync(string createCarVinCode)
  {
    // SELECT * FROM Cars WHERE VinCode = ?
    return await _context.Cars.FirstOrDefaultAsync(x => x.VinCode == createCarVinCode);
  }
  public async Task<List<CarWithTotalMileage>> GetAllWithMileageAsync()
  {
    return await _context.Cars
      .Select(car => new CarWithTotalMileage
      {
        Car = car,
        TotalMileage = _context.Mileages
          .Where(mileage => mileage.CarId == car.Id)
          .Sum(mileage => mileage.Distance)
      })
      .ToListAsync();
  }
}