using System;
using Microsoft.EntityFrameworkCore;

namespace SDPLabs.DataAccess;

public class SDPLabsDbContext : DbContext
{
	public DbSet<Car> Cars { get; set; } = null!;
	public DbSet<Mileage> Mileages { get; set; } = null!;

	public SDPLabsDbContext()
	{
	}

	protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
	{
		optionsBuilder.UseNpgsql("Host=localhost;Database=SDPLabs;User Id=sdplabsuser;Password=sdplabsuser;Port=5441");
		base.OnConfiguring(optionsBuilder);
	}
}

[Index(nameof(VinCode), IsUnique = true)]
public class Car
{
	public long Id { get; set; }
	public string Mark { get; set; } = null!;
	public string Model { get; set; }= null!;
	public int Year { get; set; }
	public string VinCode { get; set; } = null!;
	public int Price { get; set; }
	public string Color { get; set; } = null!;
	
	public List<Mileage> Mileages { get; set; } = new List<Mileage>();
}

public class Mileage
{
	public long Id { get; set; }
	public long CarId { get; set; }
	public int Distance { get; set; } 
	public DateTime DateRecorded { get; set; }
	public Car Car { get; set; } = null!;
}