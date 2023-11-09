namespace SDPLabs.DataAccess.Interfaces;

public interface ICarRepository
{
  Task<List<Car>> GetAllAsync();
  Task<Car> AddAsync(Car car);
  Task UpdateAsync(Car existing);

}