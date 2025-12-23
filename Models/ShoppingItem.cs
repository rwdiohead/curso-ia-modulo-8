using System.ComponentModel.DataAnnotations;

namespace ShoppingListAPI.Models;

/// <summary>
/// Entidad de dominio que representa un ítem en la lista de compras
/// </summary>
public class ShoppingItem
{
    /// <summary>
    /// Identificador único del ítem
    /// </summary>
    public Guid Id { get; set; }
    
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
    /// Estado actual del ítem
    /// </summary>
    [Required(ErrorMessage = "El estado es requerido")]
    public Status Status { get; set; }
    
    /// <summary>
    /// Fecha de creación del ítem (inmutable)
    /// </summary>
    public DateTime CreatedAt { get; set; }
}
