namespace ShoppingListAPI.Models;

/// <summary>
/// Estado del ítem en la lista de compras
/// </summary>
public enum Status
{
    /// <summary>
    /// Ítem planificado para comprar
    /// </summary>
    Planned = 0,
    
    /// <summary>
    /// Ítem ya comprado
    /// </summary>
    Purchased = 1,
    
    /// <summary>
    /// Ítem sin stock disponible
    /// </summary>
    OutOfStock = 2
}
