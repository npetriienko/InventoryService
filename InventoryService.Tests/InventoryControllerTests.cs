using System.Net;
using System.Net.Http.Json;
using System.Text;
using FluentAssertions;
using FluentAssertions.Execution;
using InventoryService.Data;
using InventoryService.Models;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using Xunit.Abstractions;

namespace InventoryService.Tests;

public class InventoryControllerTests(WebApplicationFactory<Program> factory, ITestOutputHelper output) 
    : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client = factory.CreateClient();

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
    public async Task GetItemsById_SeededData_ItemReturnedAsync()
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
            item.Should().BeEquivalentTo(SeedingData.Items.First(i => i.Id == itemId));
        }
    }
    
    [Fact]
    public async Task CreateItems_NewItem_ItemCreatedAsync()
    {
        // Arrange
        var newItem = new ItemModel
        {
            Title = "New Item",
            Description = "New Description",
            Price = 5.99m, Quantity = 10
        };
        
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
            item.Should().BeEquivalentTo(newItem, options => options.Excluding(i => i.Id));
        }
    }
}