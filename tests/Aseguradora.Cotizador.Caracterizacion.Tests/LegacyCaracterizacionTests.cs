using Aseguradora.Cotizador.WinForms;

namespace Aseguradora.Cotizador.Caracterizacion.Tests;

/// <summary>
/// ANTES del refactor: las pruebas de caracterización contra el código heredado, sin modificarlo.
/// </summary>
public sealed class LegacyCaracterizacionTests : CaracterizacionBase
{
    protected override Resultado Cotizar(decimal suma, string tipo, int anioVehiculo,
        DateTime nacimiento, int siniestros, string zona, DateTime hoy)
    {
        var r = CotizadorPrimaLegacy.Cotizar(suma, tipo, anioVehiculo, nacimiento, siniestros, zona, hoy);
        return new Resultado(r.PrimaNeta, r.Iva, r.Total);
    }
}
