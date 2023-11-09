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
  
  public async Task<Car?> FindByVinAsync(string vinCode)
  {
    return await _context.Cars.FirstOrDefaultAsync(c => c.VinCode == vinCode);
  }

  public async Task<List<Car>> GetAllAsync()
  {
    return await _context.Cars.ToListAsync();
  }

  public async Task UpdateAsync(Car car)
  {
    _context.Cars.Update(car);
    await _context.SaveChangesAsync();
  }

  public async Task AddAsync(Car car)
  {
    await _context.Cars.AddAsync(car);
    await _context.SaveChangesAsync();
  }
}
