using Aseguradora.Cotizador.Aplicacion.Cotizaciones;
using Aseguradora.Cotizador.Aplicacion.Puertos;
using Aseguradora.Cotizador.Dominio;
using Aseguradora.Cotizador.Infraestructura;

namespace Aseguradora.Cotizador.Caracterizacion.Tests;

/// <summary>
/// DESPUÉS del refactor: las MISMAS pruebas de caracterización, ahora contra el caso de uso
/// (Aplicación + Dominio). No se escribió ni se modificó ningún valor esperado.
/// </summary>
public sealed class DominioCaracterizacionTests : CaracterizacionBase
{
    protected override Resultado Cotizar(decimal suma, string tipo, int anioVehiculo,
        DateTime nacimiento, int siniestros, string zona, DateTime hoy)
    {
        var casoDeUso = new CotizarPrima(new RelojFijo(hoy), new RepositorioCotizacionesEnMemoria());

        var dto = casoDeUso.Ejecutar(new CotizarPrimaComando(
            suma,
            CodigosLegados.ParseTipoVehiculo(tipo),
            anioVehiculo,
            nacimiento,
            siniestros,
            CodigosLegados.ParseZona(zona)));

        return new Resultado(dto.PrimaNeta, dto.Iva, dto.Total);
    }
}

internal sealed class RelojFijo(DateTime ahora) : IReloj
{
    public DateTime Ahora { get; } = ahora;
}
