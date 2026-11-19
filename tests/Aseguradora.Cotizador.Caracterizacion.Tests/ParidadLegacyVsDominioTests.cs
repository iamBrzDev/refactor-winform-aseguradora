using Aseguradora.Cotizador.Dominio;
using Aseguradora.Cotizador.WinForms;

namespace Aseguradora.Cotizador.Caracterizacion.Tests;

/// <summary>
/// Golden master por comparación: no hay valores esperados escritos a mano.
/// Para cada escenario del producto cartesiano de límites, Legacy y Dominio deben dar exactamente lo mismo.
/// </summary>
public sealed class ParidadLegacyVsDominioTests
{
    private static readonly DateTime Hoy = new(2026, 11, 17);
    private readonly CalculadoraPrimaAutos _calculadora = new();

    [Fact]
    public void Legacy_y_Dominio_producen_el_mismo_resultado_en_todos_los_escenarios()
    {
        var diferencias = new List<string>();
        var total = 0;

        foreach (var e in Escenarios.Todos())
        {
            total++;
            var legado = CotizadorPrimaLegacy.Cotizar(e.Suma, e.Tipo, e.AnioVehiculo, e.Nacimiento, e.Siniestros, e.Zona, Hoy);

            var solicitud = new SolicitudCotizacion(e.Suma, CodigosLegados.ParseTipoVehiculo(e.Tipo), e.AnioVehiculo,
                e.Nacimiento, e.Siniestros, CodigosLegados.ParseZona(e.Zona), Hoy);
            var nuevo = _calculadora.Calcular(solicitud);

            if (legado.PrimaNeta != nuevo.PrimaNeta || legado.Iva != nuevo.Iva || legado.Total != nuevo.Total)
            {
                diferencias.Add($"{e} -> legado {legado.PrimaNeta}/{legado.Iva}/{legado.Total} vs nuevo {nuevo.PrimaNeta}/{nuevo.Iva}/{nuevo.Total}");
            }
        }

        Assert.True(total > 5000, $"Se esperaban más de 5000 escenarios y hubo {total}.");
        Assert.True(diferencias.Count == 0,
            $"{diferencias.Count} diferencias de {total} escenarios:{Environment.NewLine}{string.Join(Environment.NewLine, diferencias.Take(20))}");
    }
}
