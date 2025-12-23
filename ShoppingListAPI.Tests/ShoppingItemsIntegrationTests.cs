using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using ShoppingListAPI.DTOs;
using ShoppingListAPI.Models;
using Xunit;

namespace ShoppingListAPI.Tests;

/// <summary>
/// Tests de integración para Shopping List API
/// Cubre los 6 escenarios definidos en ARCHITECTURE.md punto 5
/// </summary>
public class ShoppingItemsIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;
    private readonly JsonSerializerOptions _jsonOptions;

    public ShoppingItemsIntegrationTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = _factory.CreateClient();
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
    }

    /// <summary>
    /// Test 1: Health Check (200 OK)
    /// </summary>
    [Fact]
    public async Task HealthCheck_ReturnsOk()
    {
        // Act
        var response = await _client.GetAsync("/health");
        
        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        
        var content = await response.Content.ReadAsStringAsync();
        Assert.Contains("Healthy", content);
    }

    /// <summary>
    /// Test 2: Crear ítem válido (201 Created)
    /// Verifica que devuelve ID
    /// </summary>
    [Fact]
    public async Task CreateValidItem_Returns201Created_WithGeneratedId()
    {
        // Arrange
        var newItem = new CreateShoppingItemDto
        {
            Name = "Leche",
            Category = "Lácteos",
            Quantity = 2,
            Status = Status.Planned
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/ShoppingItems", newItem);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        
        var createdItem = await response.Content.ReadFromJsonAsync<ShoppingItem>(_jsonOptions);
        Assert.NotNull(createdItem);
        Assert.NotEqual(Guid.Empty, createdItem.Id);
        Assert.Equal("Leche", createdItem.Name);
        Assert.Equal("Lácteos", createdItem.Category);
        Assert.Equal(2, createdItem.Quantity);
        Assert.Equal(Status.Planned, createdItem.Status);

        // Verificar Location header
        Assert.NotNull(response.Headers.Location);
    }

    /// <summary>
    /// Test 3: Crear ítem inválido (400 Bad Request)
    /// </summary>
    [Fact]
    public async Task CreateInvalidItem_Returns400BadRequest()
    {
        // Arrange - Ítem con datos inválidos (Quantity fuera de rango)
        var invalidItem = new
        {
            Name = "Producto", 
            Category = "Categoría",
            Quantity = 150 // Fuera del rango 1-100
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/ShoppingItems", invalidItem);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        
        var content = await response.Content.ReadAsStringAsync();
        Assert.NotEmpty(content);
    }

    /// <summary>
    /// Test 4: Obtener ítem inexistente (404 Not Found)
    /// </summary>
    [Fact]
    public async Task GetNonExistentItem_Returns404NotFound()
    {
        // Arrange - ID que no existe
        var nonExistentId = Guid.NewGuid();

        // Act
        var response = await _client.GetAsync($"/api/ShoppingItems/{nonExistentId}");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    /// <summary>
    /// Test 5: Ejecutar acción de compra (PATCH /purchase)
    /// Crear ítem, marcar como comprado, verificar que Status cambió a Purchased
    /// </summary>
    [Fact]
    public async Task PurchaseAction_ChangesStatusToPurchased()
    {
        // Arrange - Crear un ítem primero
        var newItem = new CreateShoppingItemDto
        {
            Name = "Pan",
            Category = "Panadería",
            Quantity = 3
        };

        var createResponse = await _client.PostAsJsonAsync("/api/ShoppingItems", newItem);
        var createdItem = await createResponse.Content.ReadFromJsonAsync<ShoppingItem>(_jsonOptions);
        Assert.NotNull(createdItem);
        Assert.Equal(Status.Planned, createdItem.Status); // Estado inicial

        // Act - Marcar como comprado
        var purchaseResponse = await _client.PatchAsync(
            $"/api/ShoppingItems/{createdItem.Id}/purchase", 
            null
        );

        // Assert
        Assert.Equal(HttpStatusCode.OK, purchaseResponse.StatusCode);
        
        var purchasedItem = await purchaseResponse.Content.ReadFromJsonAsync<ShoppingItem>(_jsonOptions);
        Assert.NotNull(purchasedItem);
        Assert.Equal(Status.Purchased, purchasedItem.Status); // Status cambió
        Assert.Equal(createdItem.Id, purchasedItem.Id);
        Assert.Equal("Pan", purchasedItem.Name);
    }

    /// <summary>
    /// Test 6: Eliminar ítem (204 No Content)
    /// Crear, eliminar, intentar obtener y verificar 404
    /// </summary>
    [Fact]
    public async Task DeleteItem_ReturnsNoContent_ThenGetReturns404()
    {
        // Arrange - Crear un ítem
        var newItem = new CreateShoppingItemDto
        {
            Name = "Huevos",
            Category = "Lácteos",
            Quantity = 12
        };

        var createResponse = await _client.PostAsJsonAsync("/api/ShoppingItems", newItem);
        var createdItem = await createResponse.Content.ReadFromJsonAsync<ShoppingItem>(_jsonOptions);
        Assert.NotNull(createdItem);

        // Act - Eliminar el ítem
        var deleteResponse = await _client.DeleteAsync($"/api/ShoppingItems/{createdItem.Id}");

        // Assert - Verificar 204 No Content
        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);

        // Act - Intentar obtener el ítem eliminado
        var getResponse = await _client.GetAsync($"/api/ShoppingItems/{createdItem.Id}");

        // Assert - Verificar 404 Not Found
        Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
    }
}
