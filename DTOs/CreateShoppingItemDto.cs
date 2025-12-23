using System.ComponentModel.DataAnnotations;
using ShoppingListAPI.Models;

namespace ShoppingListAPI.DTOs;

/// <summary>
/// DTO para crear o actualizar un ítem en la lista de compras
/// </summary>
public class CreateShoppingItemDto
{
    /// <summary>
    /// Nombre del producto
    /// </summary>
    [Required(ErrorMessage = "El nombre es requerido")]
    [MaxLength(100, ErrorMessage = "El nombre no puede exceder 100 caracteres")]
    public string Name { get; set; } = string.Empty;
    
    /// <summary>
    /// Categoría del producto (ej: Lácteos, Carnes, etc.)
    /// </summary>
    [Required(ErrorMessage = "La categoría es requerida")]
    public string Category { get; set; } = string.Empty;
    
    /// <summary>
    /// Cantidad a comprar (rango: 1-100)
    /// </summary>
    [Range(1, 100, ErrorMessage = "La cantidad debe estar entre 1 y 100")]
    public int Quantity { get; set; }
    
    /// <summary>
    /// Estado inicial del ítem (opcional, por defecto será Planned)
    /// </summary>
    public Status? Status { get; set; }
}
