using Bogus;
using InventoryService.Models;

namespace InventoryService.Tests.Builders;

/// <summary>
/// Builder class for creating instances of <see cref="ItemModel"/> with customizable properties.
/// </summary>
public class ItemBuilder
{
    private readonly Faker<ItemModel> _itemFaker;

    /// <summary>
    /// Initializes a new instance of the <see cref="ItemBuilder"/> class.
    /// </summary>
    public ItemBuilder()
    {
        _itemFaker = new Faker<ItemModel>()
            .RuleFor(i => i.Title, f => f.Commerce.ProductName())
            .RuleFor(i => i.Description, f => f.Lorem.Sentence())
            .RuleFor(i => i.Price, f => f.Random.Decimal(1, 100))
            .RuleFor(i => i.Quantity, f => f.Random.Number(1, 100));
    }

    /// <summary>
    /// Sets the title of the item.
    /// </summary>
    /// <param name="title">The title to set.</param>
    /// <returns>The current instance of <see cref="ItemBuilder"/>.</returns>
    public ItemBuilder WithTitle(string title)
    {
        _itemFaker.RuleFor(i => i.Title, _ => title);
        return this;
    }

    /// <summary>
    /// Sets the description of the item.
    /// </summary>
    /// <param name="description">The description to set.</param>
    /// <returns>The current instance of <see cref="ItemBuilder"/>.</returns>
    public ItemBuilder WithDescription(string description)
    {
        _itemFaker.RuleFor(i => i.Description, _ => description);
        return this;
    }

    /// <summary>
    /// Sets the price of the item.
    /// </summary>
    /// <param name="price">The price to set.</param>
    /// <returns>The current instance of <see cref="ItemBuilder"/>.</returns>
    public ItemBuilder WithPrice(decimal price)
    {
        _itemFaker.RuleFor(i => i.Price, _ => price);
        return this;
    }

    /// <summary>
    /// Sets the quantity of the item.
    /// </summary>
    /// <param name="quantity">The quantity to set.</param>
    /// <returns>The current instance of <see cref="ItemBuilder"/>.</returns>
    public ItemBuilder WithQuantity(int quantity)
    {
        _itemFaker.RuleFor(i => i.Quantity, _ => quantity);
        return this;
    }

    /// <summary>
    /// Builds and returns an instance of <see cref="ItemModel"/> with the specified properties.
    /// </summary>
    /// <returns>An instance of <see cref="ItemModel"/>.</returns>
    public ItemModel Build()
    {
        return _itemFaker.Generate();
    }
}