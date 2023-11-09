using Microsoft.EntityFrameworkCore;

namespace SDPLabs.DataAccess;

public class SDPLabsDbContext : DbContext
{
    public DbSet<Car> Cars { get; set; } = null!;

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
    
    public string VinCode { get; set; }
    
    public string Color { get; set; }
    
    public int Price { get; set; }
}