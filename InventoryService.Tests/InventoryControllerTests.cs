using System.Net;
using System.Net.Http.Json;
using System.Reflection;
using System.Text;
using FluentAssertions;
using FluentAssertions.Execution;
using InventoryService.Data;
using InventoryService.Models;
using InventoryService.Tests.Builders;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Xunit.Abstractions;

namespace InventoryService.Tests;

public class InventoryControllerTests(WebApplicationFactory<Program> factory, ITestOutputHelper output) 
    : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client = factory.WithWebHostBuilder(builder =>
    {
        builder.ConfigureServices(services =>
        {
            // Remove the existing DbContext registration
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<InventoryContext>));
            if (descriptor != null)
            {
                services.Remove(descriptor);
            }

            // Add DbContext using an in-memory database for testing
            services.AddDbContext<InventoryContext>(options =>
            {
                options.UseInMemoryDatabase("InMemoryDbForTesting");
            });

            // Build the service provider
            var serviceProvider = services.BuildServiceProvider();

            // Create a scope to obtain a reference to the database context
            using (var scope = serviceProvider.CreateScope())
            {
                var scopedServices = scope.ServiceProvider;
                var db = scopedServices.GetRequiredService<InventoryContext>();

                // Ensure the database is created
                db.Database.EnsureCreated();

                // Seed the database with test data if not already seeded
                var contextType = typeof(InventoryContext);
                var methodInfo = contextType.GetMethod(
                    "OnModelCreating",
                    BindingFlags.Instance | BindingFlags.NonPublic);

                if (methodInfo != null) return;
                db.Items.AddRange(SeedingData.Items);
                db.SaveChanges();
            }
        });
    }).CreateClient();

    [Fact]
    public async Task GetItems_AllStoredItems_ItemsReturnedAsync()
    {
        // Act
        var response = await _client.GetAsync("/api/items");
        
        // Assert
        using (new AssertionScope())
        {
            response.EnsureSuccessStatusCode();
            
            var items = await response.Content.ReadFromJsonAsync<List<ItemModel>>();

            items.Should().NotBeNullOrEmpty();
        }
    }
    
    [Fact]
    public async Task GetItemById_ExistingItem_ItemReturnedAsync()
    {
        // Arrange
        var itemId = 1;
        
        // Act
        var response = await _client.GetAsync($"/api/items/{itemId}");
        
        // Assert
        using (new AssertionScope())
        {
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            
            var item = await response.Content.ReadFromJsonAsync<ItemModel>();
            
            item.Should().NotBeNull();
            item.Should().BeEquivalentTo(SeedingData.Items
                .First(i => i.Id == itemId));
        }
    }
    
    [Fact]
    public async Task TryGetItemsById_NonExistingItem_ItemNotFoundAsync()
    {
        // Arrange
        var itemId = 1000;
        
        // Act
        var response = await _client.GetAsync($"/api/items/{itemId}");
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
    
    [Fact]
    public async Task CreateItems_NewItem_ItemCreatedAsync()
    {
        // Arrange
        var newItem = new ItemBuilder()
            .Build();
        
        var content = new StringContent(
            JsonConvert.SerializeObject(newItem), 
            Encoding.UTF8, 
            "application/json");

        // Act
        var response = await _client.PostAsync("/api/items", content);

        using (new AssertionScope())
        {
            response.StatusCode.Should().Be(HttpStatusCode.Created);
            
            var item = await response.Content.ReadFromJsonAsync<ItemModel>();
            
            item.Should().NotBeNull();
            item.Should().BeEquivalentTo(newItem, options => 
                options.Excluding(i => i.Id));
        }
    }
    
    [Fact]
    public async Task UpdateItem_ExistingItem_ItemUpdatedAsync()
    {
        // Arrange
        var itemId = 2;
        var updatedItem = new ItemBuilder()
            .Build();

        var content = new StringContent(
            JsonConvert.SerializeObject(updatedItem),
            Encoding.UTF8,
            "application/json");

        // Act
        var response = await _client.PutAsync($"/api/items/{itemId}", content);

        // Assert
        using (new AssertionScope())
        {
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var item = await response.Content.ReadFromJsonAsync<ItemModel>();

            item.Should().NotBeNull();
            item.Should().BeEquivalentTo(updatedItem, options => 
                options.Excluding(i => i.Id));
        }
    }
    
    [Fact]
    public async Task TryUpdateItem_NonExistingItem_ItemNotFoundAsync()
    {
        // Arrange
        var itemId = 100;
        var updatedItem = new ItemModel
        {
            Title = "Updated Item",
            Description = "Updated Description",
            Price = 10.99m,
            Quantity = 5
        };

        var content = new StringContent(
            JsonConvert.SerializeObject(updatedItem),
            Encoding.UTF8,
            "application/json");

        // Act
        var response = await _client.PutAsync($"/api/items/{itemId}", content);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeleteItem_ExistingItem_ItemDeletedAsync()
    {
        // Arrange
        var itemId = 3;

        // Act
        var response = await _client.DeleteAsync($"/api/items/{itemId}");
        
        // Assert
        using (new AssertionScope())
        {
            response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        
            var getItemResponse = await _client.GetAsync($"/api/items/{itemId}");
            getItemResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }
        
    }

    [Fact]
    public async Task TryDeleteItem_NonExistingItem_ItemNotFoundAsync()
    {
        // Arrange
        var itemId = 100;

        // Act
        var response = await _client.DeleteAsync($"/api/items/{itemId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}