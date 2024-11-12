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
        modelBuilder.Entity<ItemModel>().HasData(
            new ItemModel
            {
                Id = 1,
                Title = "Item 1",
                Description = "Description for Item 1",
                Price = 10.99m,
                Quantity = 5
            },
            new ItemModel
            {
                Id = 2,
                Title = "Item 2",
                Description = "Description for Item 2",
                Price = 15.99m,
                Quantity = 10
            },
            new ItemModel
            {
                Id = 3,
                Title = "Item 3",
                Description = "Description for Item 3",
                Price = 20.99m,
                Quantity = 15
            }
        );
    }
}