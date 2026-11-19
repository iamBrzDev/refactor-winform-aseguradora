using System;
using System.Configuration;
using System.Diagnostics;

namespace Aseguradora.Cotizador.WinForms
{
    /// <summary>
    /// Punto de extensión del patrón Strangler. Ticket: STRANGLER-001.
    /// Interruptor del strangler. Se lee en CADA cotización (RefreshSection), así que operaciones puede
    /// cambiar Cotizador.exe.config y el siguiente clic ya usa el motor indicado, sin reiniciar ni redeploy.
    ///   Legacy  -> solo el cálculo heredado (valor por defecto y plan de reversa).
    ///   Sombra  -> se muestra el heredado y se calcula también el nuevo; cualquier diferencia se registra
    ///              en sombra.log (TextWriterTraceListener configurado en App.config).
    ///   Nuevo   -> se muestra el nuevo; si el motor nuevo lanza una excepción se registra y se usa Legacy.
    /// </summary>
    internal static class SelectorMotor
    {
        public static ResultadoCotizacionLegacy Cotizar(decimal suma, string tipo, int anioVehiculo,
            DateTime nacimiento, int siniestros, string zona, DateTime hoy)
        {
            ConfigurationManager.RefreshSection("appSettings");
            string motor = ConfigurationManager.AppSettings["MotorCotizacion"] ?? "Legacy";

            switch (motor)
            {
                case "Nuevo":
                    try
                    {
                        return MotorCotizacionNuevo.Cotizar(suma, tipo, anioVehiculo, nacimiento, siniestros, zona, hoy);
                    }
                    catch (Exception ex)
                    {
                        // La operación no se interrumpe: se registra y se responde con el motor heredado.
                        Trace.TraceError("[NUEVO] Error en motor nuevo; se usa Legacy. " + ex);
                        return CotizadorPrimaLegacy.Cotizar(suma, tipo, anioVehiculo, nacimiento, siniestros, zona, hoy);
                    }

                case "Sombra":
                    ResultadoCotizacionLegacy legado = CotizadorPrimaLegacy.Cotizar(suma, tipo, anioVehiculo, nacimiento, siniestros, zona, hoy);
                    try
                    {
                        ResultadoCotizacionLegacy nuevo = MotorCotizacionNuevo.Cotizar(suma, tipo, anioVehiculo, nacimiento, siniestros, zona, hoy);
                        if (legado.PrimaNeta != nuevo.PrimaNeta || legado.Iva != nuevo.Iva || legado.Total != nuevo.Total)
                        {
                            Trace.TraceWarning(string.Format(
                                "[SOMBRA] Diferencia suma={0} tipo={1} anio={2} nac={3:yyyy-MM-dd} sin={4} zona={5} legado={6}/{7} nuevo={8}/{9}",
                                suma, tipo, anioVehiculo, nacimiento, siniestros, zona,
                                legado.PrimaNeta, legado.Total, nuevo.PrimaNeta, nuevo.Total));
                        }
                    }
                    catch (Exception ex)
                    {
                        // En modo sombra el motor nuevo nunca puede afectar al usuario.
                        Trace.TraceError("[SOMBRA] Error en motor nuevo: " + ex.Message);
                    }
                    return legado;

                default:
                    return CotizadorPrimaLegacy.Cotizar(suma, tipo, anioVehiculo, nacimiento, siniestros, zona, hoy);
            }
        }
    }
}
