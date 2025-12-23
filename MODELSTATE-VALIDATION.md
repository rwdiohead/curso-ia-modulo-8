# ModelState Validation en .NET 8

## Comportamiento Automático con [ApiController]

En .NET 8, cuando se usa el atributo `[ApiController]` en un controlador, la validación de ModelState es **automática**.

### ¿Qué hace automáticamente?

1. **Valida el modelo** usando DataAnnotations antes de que el método del controlador se ejecute
2. **Retorna automáticamente 400 Bad Request** si ModelState.IsValid == false
3. **Incluye los detalles del error** en formato ProblemDetails (RFC 7807)

### Ejemplo de Validación Automática

```csharp
[ApiController]
[Route("api/[controller]")]
public class ShoppingItemsController : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<ShoppingItem>> Create([FromBody] CreateShoppingItemDto dto)
    {
        // NO es necesario validar manualmente:
        // if (!ModelState.IsValid) return BadRequest(ModelState);
        
        // La validación ya ocurrió automáticamente gracias a [ApiController]
        var item = await _service.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = item.Id }, item);
    }
}
```

### Formato de Respuesta de Error (Automático)

Cuando se envía un request inválido, .NET 8 retorna automáticamente:

**Status Code:** 400 Bad Request

**Formato ProblemDetails:**
```json
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.1",
  "title": "One or more validation errors occurred.",
  "status": 400,
  "traceId": "00-xxxxx",
  "errors": {
    "Name": [
      "El nombre es requerido"
    ],
    "Quantity": [
      "La cantidad debe estar entre 1 y 100"
    ]
  }
}
```

### Personalización (Opcional)

Si necesitas personalizar el formato de respuesta de validación:

```csharp
builder.Services.AddControllers()
    .ConfigureApiBehaviorOptions(options =>
    {
        options.InvalidModelStateResponseFactory = context =>
        {
            return new BadRequestObjectResult(new
            {
                error = "Datos inválidos",
                code = 400,
                details = context.ModelState
            });
        };
    });
```

## Verificación en Nuestra API

### Endpoints que Validan Automáticamente

- **POST /api/ShoppingItems** - Valida CreateShoppingItemDto
- **PUT /api/ShoppingItems/{id}** - Valida CreateShoppingItemDto

### Validaciones Activas (DataAnnotations)

Desde `CreateShoppingItemDto.cs`:
- `Name`: [Required], [MaxLength(100)]
- `Category`: [Required]
- `Quantity`: [Range(1, 100)]

### Prueba Manual

Para probar la validación automática:

1. Enviar POST a `/api/ShoppingItems` con body vacío:
   ```json
   {}
   ```
   **Resultado:** 400 Bad Request con detalles de errores

2. Enviar POST con Quantity inválida:
   ```json
   {
     "name": "Leche",
     "category": "Lácteos",
     "quantity": 150
   }
   ```
   **Resultado:** 400 Bad Request - "La cantidad debe estar entre 1 y 100"

## Conclusión

✅ **ModelState validation está habilitada automáticamente** gracias a `[ApiController]`  
✅ **No requiere código adicional** en los métodos del controlador  
✅ **Retorna 400 Bad Request** con detalles de error automáticamente  
✅ **Sigue el estándar ProblemDetails** (RFC 7807)

**Cumple con los requisitos de ARCHITECTURE.md:**
- Validaciones fallidas → 400 Bad Request ✅
- Formato consistente de errores ✅
