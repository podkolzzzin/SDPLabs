namespace SDPLabs.DataAccess.Interfaces;

public interface ICarRepository
{
  Task<List<Car>> GetAllAsync();
  Task<Car> AddAsync(Car car);
  Task UpdateAsync(Car existing);
  Task<Car?> FindByVinCodeAsync(string createCarVinCode);
  Task<Mileage> AddMileageAsync(Mileage mileage);
  Task<List<Mileage>> GetAllMileagesAsync();
  
}