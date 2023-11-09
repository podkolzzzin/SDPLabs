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
    return await _context.Cars.ToListAsync();
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
}