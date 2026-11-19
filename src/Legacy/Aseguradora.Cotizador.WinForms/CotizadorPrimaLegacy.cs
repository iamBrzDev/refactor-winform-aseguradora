using System;
using Aseguradora.Tarifas;

namespace Aseguradora.Cotizador.WinForms
{
    /// <summary>
    /// COSTURA (seam). Código MOVIDO tal cual desde FrmCotizacionAuto.btnCalcular_Click.
    /// Únicos cambios:
    ///   1. Las entradas llegan por parámetro en lugar de leerse de los controles.
    ///   2. DateTime.Now se reemplaza por el parámetro <paramref name="hoy"/>
    ///      (antes se leía dos veces; ahora una sola, lo que elimina la carrera de medianoche del 31/12).
    /// No corregir nada aquí: las pruebas de caracterización fijan este comportamiento.
    /// Este archivo se compila también dentro de las pruebas (archivo enlazado), por eso no depende de WinForms.
    /// </summary>
    public static class CotizadorPrimaLegacy
    {
        public static ResultadoCotizacionLegacy Cotizar(decimal suma, string tipo, int anioVehiculo,
            DateTime nacimiento, int siniestros, string zona, DateTime hoy)
        {
            // Prima base según tabla de tarifas (módulo VB)
            decimal prima = suma * ModTarifas.ObtenerTasaBase(tipo);

            // Recargo por edad del conductor
            int edad = hoy.Year - nacimiento.Year;
            if (edad < 25)
            {
                prima = prima * 1.25m;
            }
            else if (edad > 65)
            {
                prima = prima * 1.15m;
            }

            // Recargo por antigüedad del vehículo (manual de suscripción: "10 años o más")
            int antiguedad = hoy.Year - anioVehiculo;
            if (antiguedad > 10)
            {
                prima = prima * 1.20m;
            }

            // Ajuste por zona
            prima = prima * ModTarifas.FactorZona(zona);

            // Prima mínima de la póliza
            if (prima < 150)
            {
                prima = 150;
            }

            // Bonificación / recargo por siniestralidad (últimos 3 años)
            if (siniestros == 0)
            {
                prima = prima * 0.90m;
            }
            else if (siniestros >= 3)
            {
                prima = prima * 1.30m;
            }

            prima = Math.Round(prima, 2);
            decimal iva = Math.Round(prima * 0.13m, 2);
            decimal total = prima + iva;

            return new ResultadoCotizacionLegacy(prima, iva, total);
        }
    }

    public sealed class ResultadoCotizacionLegacy
    {
        public ResultadoCotizacionLegacy(decimal primaNeta, decimal iva, decimal total)
        {
            PrimaNeta = primaNeta;
            Iva = iva;
            Total = total;
        }

        public decimal PrimaNeta { get; }
        public decimal Iva { get; }
        public decimal Total { get; }
    }
}
