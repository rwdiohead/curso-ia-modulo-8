using System.Collections.Concurrent;
using ShoppingListAPI.DTOs;
using ShoppingListAPI.Models;

namespace ShoppingListAPI.Services;

/// <summary>
/// Implementación del servicio de lista de compras usando persistencia en memoria
/// </summary>
public class InMemoryShoppingService : IShoppingService
{
    private readonly ConcurrentDictionary<Guid, ShoppingItem> _items;

    public InMemoryShoppingService()
    {
        _items = new ConcurrentDictionary<Guid, ShoppingItem>();
    }

    /// <inheritdoc />
    public Task<IEnumerable<ShoppingItem>> GetAllAsync()
    {
        return Task.FromResult(_items.Values.AsEnumerable());
    }

    /// <inheritdoc />
    public Task<ShoppingItem?> GetByIdAsync(Guid id)
    {
        _items.TryGetValue(id, out var item);
        return Task.FromResult(item);
    }

    /// <inheritdoc />
    public Task<ShoppingItem> CreateAsync(CreateShoppingItemDto dto)
    {
        var item = new ShoppingItem
        {
            Id = Guid.NewGuid(),
            Name = dto.Name,
            Category = dto.Category,
            Quantity = dto.Quantity,
            Status = dto.Status ?? Status.Planned, // Por defecto Planned si no se especifica
            CreatedAt = DateTime.UtcNow
        };

        if (!_items.TryAdd(item.Id, item))
        {
            throw new InvalidOperationException("No se pudo agregar el ítem a la colección");
        }

        return Task.FromResult(item);
    }

    /// <inheritdoc />
    public Task<ShoppingItem?> UpdateAsync(Guid id, CreateShoppingItemDto dto)
    {
        if (!_items.TryGetValue(id, out var existingItem))
        {
            return Task.FromResult<ShoppingItem?>(null);
        }

        var updatedItem = new ShoppingItem
        {
            Id = existingItem.Id,
            Name = dto.Name,
            Category = dto.Category,
            Quantity = dto.Quantity,
            Status = dto.Status ?? existingItem.Status,
            CreatedAt = existingItem.CreatedAt // Mantener fecha original (inmutable)
        };

        if (!_items.TryUpdate(id, updatedItem, existingItem))
        {
            // Si falla, intentar nuevamente obteniendo el valor actual
            return UpdateAsync(id, dto);
        }

        return Task.FromResult<ShoppingItem?>(updatedItem);
    }

    /// <inheritdoc />
    public Task<bool> DeleteAsync(Guid id)
    {
        var removed = _items.TryRemove(id, out _);
        return Task.FromResult(removed);
    }

    /// <inheritdoc />
    public Task<ShoppingItem?> MarkAsPurchasedAsync(Guid id)
    {
        if (!_items.TryGetValue(id, out var existingItem))
        {
            return Task.FromResult<ShoppingItem?>(null);
        }

        var updatedItem = new ShoppingItem
        {
            Id = existingItem.Id,
            Name = existingItem.Name,
            Category = existingItem.Category,
            Quantity = existingItem.Quantity,
            Status = Status.Purchased, // Cambiar status a Purchased
            CreatedAt = existingItem.CreatedAt
        };

        if (!_items.TryUpdate(id, updatedItem, existingItem))
        {
            // Si falla, intentar nuevamente
            return MarkAsPurchasedAsync(id);
        }

        return Task.FromResult<ShoppingItem?>(updatedItem);
    }
}
