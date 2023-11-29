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
  
  public async Task<List<Mileage>> GetAllMileagesAsync()
  {
    return await _context.Mileages
      .ToListAsync();
  }

}

public class MileageRepository : IMileageRepository
{
  private readonly SDPLabsDbContext _context;

  public MileageRepository(SDPLabsDbContext context)
  {
    _context = context;
  }

  public async Task<Mileage> AddAsync(Mileage mileage)
  {
    _context.Add(mileage);
    await _context.SaveChangesAsync();
    return mileage;
  }

  public async Task<List<Mileage>> FindByCarIdAsync(long carId)
  {
    return await _context.Mileages
      .Where(m => m.CarId == carId)
      .ToListAsync();
  }

  public async Task UpdateAsync(Mileage mileage)
  {
    _context.Update(mileage);
    await _context.SaveChangesAsync();
  }

  public async Task DeleteAsync(Mileage mileage)
  {
    _context.Remove(mileage);
    await _context.SaveChangesAsync();
  }

  public async Task<List<Mileage>> GetAllMileagesAsync()
  {
    return await _context.Mileages
      .ToListAsync();
  }
}