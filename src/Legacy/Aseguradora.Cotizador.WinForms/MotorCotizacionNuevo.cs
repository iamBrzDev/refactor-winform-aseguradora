using System;
using Aseguradora.Cotizador.Dominio;

namespace Aseguradora.Cotizador.WinForms
{
    /// <summary>
    /// Punto de extensión del patrón Strangler. Ticket: STRANGLER-001.
    /// Adaptador del borde WinForms hacia el dominio nuevo (netstandard2.0).
    /// Llama directo al dominio (no pasa por Aplicacion): no necesita persistir ni IReloj, la fecha llega por parámetro.
    /// Devuelve el mismo tipo que el motor heredado para que el formulario no note la diferencia.
    /// La persistencia sigue siendo la del formulario en esta fase del strangler.
    /// </summary>
    internal static class MotorCotizacionNuevo
    {
        private static readonly CalculadoraPrimaAutos Calculadora = new CalculadoraPrimaAutos();

        public static ResultadoCotizacionLegacy Cotizar(decimal suma, string tipo, int anioVehiculo,
            DateTime nacimiento, int siniestros, string zona, DateTime hoy)
        {
            var solicitud = new SolicitudCotizacion(
                suma,
                CodigosLegados.ParseTipoVehiculo(tipo),
                anioVehiculo,
                nacimiento,
                siniestros,
                CodigosLegados.ParseZona(zona),
                hoy);

            PrimaCalculada prima = Calculadora.Calcular(solicitud);
            return new ResultadoCotizacionLegacy(prima.PrimaNeta, prima.Iva, prima.Total);
        }
    }
}
