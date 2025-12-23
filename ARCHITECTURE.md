# ARCHITECTURE.md - Shopping List API

## 1. Contexto del Negocio
Sistema de gestión para una "Lista de Compras Inteligente".
**Justificación:** Permite a los usuarios gestionar su despensa y lista de supermercado para evitar olvidar productos esenciales o comprar duplicados.
**Solución:** API RESTful que administra ítems, cantidades y estados de compra.

## 2. Stack Tecnológico
- **Framework:** .NET 8 (C#)
- **Tipo de Proyecto:** ASP.NET Core Web API.
- **Persistencia:** En memoria (Singleton Service o ConcurrentDictionary).
- **Testing:** xUnit + `Microsoft.AspNetCore.Mvc.Testing` (Integration Tests).
- **Documentación:** Swagger / OpenAPI.

## 3. Estructura de la Entidad (ShoppingItem)
Debe cumplir estrictamente con:
- **Id:** `Guid` (Identificador único).
- **Name:** `string` (Requerido, Max 100 chars). Nombre del producto.
- **Category:** `string` (Requerido, "Lácteos", "Carnes", etc.).
- **Quantity:** `int` (Rango: 1 - 100). Cantidad a comprar.
- **Status:** `Enum` (Planned, Purchased, OutOfStock). Control de estado.
- **CreatedAt:** `DateTime`. Fecha de creación (inmutable).

## 4. Reglas de la API (Restricciones Técnicas)
1. **Rutas:** Todas deben iniciar con `/api/[controller]`.
2. **Endpoints Mínimos:**
   - `GET /health` (Health Check).
   - CRUD completo para `ShoppingItem`.
   - **Action Endpoint:** `PATCH /api/shoppingitems/{id}/purchase` (Marca el ítem como comprado sin enviar todo el payload).
3. **Manejo de Errores:**
   - Uso de `ProblemDetails` o formato consistente `{ "error": "mensaje", "code": 400 }`.
   - Validaciones fallidas -> 400 Bad Request.
   - Recurso no encontrado -> 404 Not Found.
   - Error servidor -> 500 Internal Server Error.
4. **Validaciones:** FluentValidation o DataAnnotations.

## 5. Estrategia de Pruebas (xUnit)
Las pruebas deben ser de integración (llamando a la API real en memoria via `WebApplicationFactory`):
1. Health Check (200 OK).
2. Crear item válido (201 Created).
3. Crear item inválido (400 Bad Request).
4. Obtener item inexistente (404 Not Found).
5. Ejecutar acción de compra (Verificar cambio de estado).
6. Eliminar item (204 No Content y verificar 404 subsiguiente).