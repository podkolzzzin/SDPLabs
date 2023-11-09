namespace SDPLabs.DataAccess.Interfaces;

public interface ICarRepository
{
    Task AddAsync(Car car);
    Task UpdateAsync(Car car);
    Task<Car?> FindByVinAsync(string vinCode);
    Task<List<Car>> GetAllAsync();
}