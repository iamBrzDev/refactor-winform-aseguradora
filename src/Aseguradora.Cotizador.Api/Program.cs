using Aseguradora.Cotizador.Api;
using Aseguradora.Cotizador.Aplicacion.Cotizaciones;
using Aseguradora.Cotizador.Aplicacion.Puertos;
using Aseguradora.Cotizador.Infraestructura;

var builder = WebApplication.CreateBuilder(args);

// Composición: el único lugar donde los bordes conocen las implementaciones concretas.
builder.Services.AddSingleton<IReloj, RelojSistema>();
builder.Services.AddSingleton<IRepositorioCotizaciones, RepositorioCotizacionesEnMemoria>();
builder.Services.AddScoped<CotizarPrima>();
builder.Services.AddScoped<ConsultarCotizacion>();
builder.Services.AddProblemDetails();
builder.Services.AddOpenApi();

var app = builder.Build();

app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapCotizaciones();
app.MapGet("/health", () => Results.Ok(new { estado = "ok" })).ExcludeFromDescription();

app.Run();

/// <summary>Expuesto para WebApplicationFactory en las pruebas de integración.</summary>
public partial class Program { }
