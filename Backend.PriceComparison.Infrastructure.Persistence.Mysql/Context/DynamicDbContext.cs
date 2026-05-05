using Microsoft.EntityFrameworkCore;

namespace Backend.PriceComparison.Infrastructure.Persistence.Mysql.Context;

public class DynamicDbContext(string _connectionString) : DbContext
{
   
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseMySql(_connectionString, ServerVersion.AutoDetect(_connectionString));
        }
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
     
    }
}
