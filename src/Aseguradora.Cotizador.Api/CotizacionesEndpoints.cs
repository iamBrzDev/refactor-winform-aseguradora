using Aseguradora.Cotizador.Aplicacion.Cotizaciones;
using Aseguradora.Cotizador.Dominio;

namespace Aseguradora.Cotizador.Api;

/// <summary>Borde HTTP: traduce JSON ⇄ comandos de aplicación. Sin reglas de negocio aquí.</summary>
public static class CotizacionesEndpoints
{
    public static IEndpointRouteBuilder MapCotizaciones(this IEndpointRouteBuilder app)
    {
        var grupo = app.MapGroup("/api/v1/cotizaciones").WithTags("Cotizaciones");

        grupo.MapPost("/", Cotizar)
            .WithName("CotizarPrima")
            .WithSummary("Calcula y registra la prima de un seguro de auto.");

        grupo.MapGet("/{id:guid}", Consultar)
            .WithName("ConsultarCotizacion");

        return app;
    }

    private static IResult Cotizar(CotizacionRequest request, CotizarPrima casoDeUso)
    {
        var errores = new Dictionary<string, string[]>();

        if (!CodigosLegados.TryTipoVehiculo(request.TipoVehiculo, out var tipo))
        {
            errores["tipoVehiculo"] = ["Valores permitidos: SEDAN, PICKUP, MOTO, MICROBUS."];
        }

        if (!CodigosLegados.TryZona(request.Zona, out var zona))
        {
            errores["zona"] = ["Valores permitidos: URBANA, RURAL."];
        }

        // Obligatorios: sin ellos el JSON se deserializaba como 0001-01-01 / 0 y se respondía 201.
        var fechaNacimiento = request.FechaNacimientoConductor.GetValueOrDefault();
        if (fechaNacimiento == default)
        {
            errores["fechaNacimientoConductor"] = ["Obligatoria. Formato yyyy-MM-dd; no se admite 0001-01-01."];
        }

        var anioVehiculo = request.AnioVehiculo.GetValueOrDefault();
        if (anioVehiculo <= 0)
        {
            errores["anioVehiculo"] = ["Obligatorio. Debe ser un año válido (entero mayor que cero)."];
        }

        if (errores.Count > 0)
        {
            return TypedResults.ValidationProblem(errores);
        }

        try
        {
            var dto = casoDeUso.Ejecutar(new CotizarPrimaComando(
                request.SumaAsegurada,
                tipo,
                anioVehiculo,
                fechaNacimiento,
                request.SiniestrosUltimos3Anios,
                zona));

            return TypedResults.Created($"/api/v1/cotizaciones/{dto.Id}", dto);
        }
        catch (ExcepcionDominio ex)
        {
            return TypedResults.ValidationProblem(new Dictionary<string, string[]>
            {
                ["solicitud"] = [ex.Message],
            });
        }
    }

    private static IResult Consultar(Guid id, ConsultarCotizacion casoDeUso)
    {
        var dto = casoDeUso.Ejecutar(id);
        return dto is null ? TypedResults.NotFound() : TypedResults.Ok(dto);
    }
}

/// <summary>
/// Contrato público de la API (usa los mismos códigos que el sistema heredado).
/// <c>AnioVehiculo</c> y <c>FechaNacimientoConductor</c> son nullable para poder detectar que faltan y responder 400.
/// </summary>
public sealed record CotizacionRequest(
    decimal SumaAsegurada,
    string? TipoVehiculo,
    int? AnioVehiculo,
    DateTime? FechaNacimientoConductor,
    int SiniestrosUltimos3Anios,
    string? Zona);
