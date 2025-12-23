using Microsoft.OpenApi.Models;
using ShoppingListAPI.Middleware;
using ShoppingListAPI.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Configuración mejorada de Swagger
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Shopping List API",
        Version = "v1",
        Description = "API RESTful para gestión de lista de compras inteligente",
        Contact = new OpenApiContact
        {
            Name = "Soporte API",
            Email = "support@shoppinglist.com"
        }
    });

    // Opcional: Agregar comentarios XML si tienes un archivo XML generado
    // var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    // var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    // options.IncludeXmlComments(xmlPath);
});

// Registrar InMemoryShoppingService como Singleton
builder.Services.AddSingleton<IShoppingService, InMemoryShoppingService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
// Middleware de manejo global de excepciones (debe ir primero)
app.UseGlobalExceptionHandler();

// Habilitar Swagger en desarrollo
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Shopping List API V1");
        options.RoutePrefix = "swagger"; // URL: /swagger
        options.DocumentTitle = "Shopping List API - Swagger UI";
    });
}

app.UseHttpsRedirection();

app.UseAuthorization();

// Mapear controladores
app.MapControllers();

// Health Check Endpoint
app.MapGet("/health", () =>
{
    return Results.Ok(new { status = "Healthy" });
})
.WithName("HealthCheck")
.WithTags("Health")
.WithOpenApi();

app.Run();

// Hacer Program accesible para WebApplicationFactory en tests de integración
public partial class Program { }
