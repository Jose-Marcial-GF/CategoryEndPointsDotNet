using Microsoft.EntityFrameworkCore;

public class AplicationDbContext: DbContext
{
    public AplicationDbContext(DbContextOptions<AplicationDbContext> options): base (options)
    {
        
    }

    public DbSet<Category> Catetories { get; set; }
    
}