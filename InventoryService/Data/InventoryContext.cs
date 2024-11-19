using InventoryService.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace InventoryService.Data;

public class InventoryContext(DbContextOptions<InventoryContext> options, IConfiguration configuration)
    : DbContext(options)
{
    public DbSet<ItemModel> Items { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            optionsBuilder.UseNpgsql(connectionString);
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ItemModel>().HasData(SeedingData.Items);
    }
}