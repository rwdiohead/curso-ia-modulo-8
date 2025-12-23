using Microsoft.AspNetCore.Mvc;
using ShoppingListAPI.DTOs;
using ShoppingListAPI.Models;
using ShoppingListAPI.Services;

namespace ShoppingListAPI.Controllers;

/// <summary>
/// Controlador para gestionar ítems de la lista de compras
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class ShoppingItemsController : ControllerBase
{
    private readonly IShoppingService _service;

    public ShoppingItemsController(IShoppingService service)
    {
        _service = service;
    }

    /// <summary>
    /// Obtiene todos los ítems de la lista de compras
    /// </summary>
    /// <returns>Lista de todos los ítems</returns>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<ShoppingItem>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<ShoppingItem>>> GetAll()
    {
        var items = await _service.GetAllAsync();
        return Ok(items);
    }

    /// <summary>
    /// Obtiene un ítem específico por su ID
    /// </summary>
    /// <param name="id">ID del ítem</param>
    /// <returns>El ítem solicitado</returns>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ShoppingItem), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ShoppingItem>> GetById(Guid id)
    {
        var item = await _service.GetByIdAsync(id);
        
        if (item == null)
        {
            return NotFound(new { error = "Ítem no encontrado", code = 404 });
        }

        return Ok(item);
    }

    /// <summary>
    /// Crea un nuevo ítem en la lista de compras
    /// </summary>
    /// <param name="dto">Datos del ítem a crear</param>
    /// <returns>El ítem creado</returns>
    [HttpPost]
    [ProducesResponseType(typeof(ShoppingItem), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ShoppingItem>> Create([FromBody] CreateShoppingItemDto dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(new { error = "Datos inválidos", code = 400, details = ModelState });
        }

        var item = await _service.CreateAsync(dto);
        
        return CreatedAtAction(
            nameof(GetById), 
            new { id = item.Id }, 
            item
        );
    }

    /// <summary>
    /// Actualiza un ítem existente
    /// </summary>
    /// <param name="id">ID del ítem a actualizar</param>
    /// <param name="dto">Nuevos datos del ítem</param>
    /// <returns>El ítem actualizado</returns>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(ShoppingItem), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ShoppingItem>> Update(Guid id, [FromBody] CreateShoppingItemDto dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(new { error = "Datos inválidos", code = 400, details = ModelState });
        }

        var item = await _service.UpdateAsync(id, dto);
        
        if (item == null)
        {
            return NotFound(new { error = "Ítem no encontrado", code = 404 });
        }

        return Ok(item);
    }

    /// <summary>
    /// Elimina un ítem de la lista de compras
    /// </summary>
    /// <param name="id">ID del ítem a eliminar</param>
    /// <returns>No Content si se eliminó exitosamente</returns>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id)
    {
        var deleted = await _service.DeleteAsync(id);
        
        if (!deleted)
        {
            return NotFound(new { error = "Ítem no encontrado", code = 404 });
        }

        return NoContent();
    }

    /// <summary>
    /// Marca un ítem como comprado (cambia el status a Purchased)
    /// </summary>
    /// <param name="id">ID del ítem a marcar como comprado</param>
    /// <returns>El ítem actualizado</returns>
    [HttpPatch("{id}/purchase")]
    [ProducesResponseType(typeof(ShoppingItem), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ShoppingItem>> MarkAsPurchased(Guid id)
    {
        var item = await _service.MarkAsPurchasedAsync(id);
        
        if (item == null)
        {
            return NotFound(new { error = "Ítem no encontrado", code = 404 });
        }

        return Ok(item);
    }
}
