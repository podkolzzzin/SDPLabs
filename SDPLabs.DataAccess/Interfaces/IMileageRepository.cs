using SDPLabs.DataAccess.Interfaces;

namespace SDPLabs.DataAccess.Interfaces;

public interface IMileageRepository
{
	Task<Mileage> AddAsync(Mileage mileage);

	Task<List<Mileage>> FindByCarIdAsync(long carId);

	Task UpdateAsync(Mileage mileage);

	Task<List<Mileage>> GetAllMileagesAsync();
}