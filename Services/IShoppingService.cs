using ShoppingListAPI.Models;
using ShoppingListAPI.DTOs;

namespace ShoppingListAPI.Services;

/// <summary>
/// Interfaz del servicio para gestionar ítems de la lista de compras
/// </summary>
public interface IShoppingService
{
    /// <summary>
    /// Obtiene todos los ítems de la lista de compras
    /// </summary>
    Task<IEnumerable<ShoppingItem>> GetAllAsync();
    
    /// <summary>
    /// Obtiene un ítem por su ID
    /// </summary>
    /// <param name="id">ID del ítem</param>
    /// <returns>El ítem si existe, null si no se encuentra</returns>
    Task<ShoppingItem?> GetByIdAsync(Guid id);
    
    /// <summary>
    /// Crea un nuevo ítem en la lista de compras
    /// </summary>
    /// <param name="dto">Datos del ítem a crear</param>
    /// <returns>El ítem creado</returns>
    Task<ShoppingItem> CreateAsync(CreateShoppingItemDto dto);
    
    /// <summary>
    /// Actualiza un ítem existente
    /// </summary>
    /// <param name="id">ID del ítem a actualizar</param>
    /// <param name="dto">Nuevos datos del ítem</param>
    /// <returns>El ítem actualizado si existe, null si no se encuentra</returns>
    Task<ShoppingItem?> UpdateAsync(Guid id, CreateShoppingItemDto dto);
    
    /// <summary>
    /// Elimina un ítem de la lista
    /// </summary>
    /// <param name="id">ID del ítem a eliminar</param>
    /// <returns>true si se eliminó, false si no se encontró</returns>
    Task<bool> DeleteAsync(Guid id);
    
    /// <summary>
    /// Marca un ítem como comprado (cambia el status a Purchased)
    /// </summary>
    /// <param name="id">ID del ítem a marcar como comprado</param>
    /// <returns>El ítem actualizado si existe, null si no se encuentra</returns>
    Task<ShoppingItem?> MarkAsPurchasedAsync(Guid id);
}
