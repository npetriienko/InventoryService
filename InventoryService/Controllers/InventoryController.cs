using AutoMapper;
using InventoryService.Data;
using InventoryService.Models;
using InventoryService.Models.Responses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InventoryService.Controllers;

[ApiController]
[Route("api/items")]
public class InventoryController(InventoryContext inventoryContext, IMapper mapper) : ControllerBase
{
    [HttpGet("{id:int}")]
    public async Task<ActionResult<ItemContractResponse>> GetItemAsync(int id)
    {
        var item = await inventoryContext.Items.FindAsync(id);
        
        if(item == null)
        {
            return NotFound();
        }

        var response = mapper.Map<ItemContractResponse>(item);
        return Ok(response);
    }
    
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ItemContractResponse>>> GetItemsAsync()
    {
        var items = await inventoryContext.Items.ToListAsync();
        var response = mapper.Map<List<ItemContractResponse>>(items);
        return Ok(response);
    }
    
    [HttpPost]
    public async Task<ActionResult<ItemContractResponse>> CreateItemAsync(ItemContractResponse itemContract)
    {
        var item = new ItemModel
        {
            Title = itemContract.Title,
            Description = itemContract.Description,
            Price = itemContract.Price,
            Quantity = itemContract.Quantity
        };

        await inventoryContext.Items.AddAsync(item);
        await inventoryContext.SaveChangesAsync();

        return Created(nameof(CreateItemAsync), mapper.Map<ItemContractResponse>(item));
    }
    
    [HttpPut("{id:int}")]
    public async Task<ActionResult<ItemContractResponse>> UpdateItemAsync(int id, ItemContractResponse itemContract)
    {
        var item = await inventoryContext.Items.FindAsync(id);
        
        if(item == null)
        {
            return NotFound();
        }

        item.Title = itemContract.Title;
        item.Description = itemContract.Description;
        item.Price = itemContract.Price;
        item.Quantity = itemContract.Quantity;

        await inventoryContext.SaveChangesAsync();

        return Ok(mapper.Map<ItemContractResponse>(item));
    }
    
    [HttpDelete("{id:int}")]
    public async Task<ActionResult> DeleteItemAsync(int id)
    {
        var item = await inventoryContext.Items.FindAsync(id);
        
        if(item == null)
        {
            return NotFound();
        }

        inventoryContext.Items.Remove(item);
        await inventoryContext.SaveChangesAsync();

        return NoContent();
    }
}