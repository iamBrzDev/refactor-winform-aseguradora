using System;

namespace Aseguradora.Cotizador.Dominio
{
    /// <summary>
    /// Servicio de dominio: calcula la prima de un seguro de auto.
    /// El orden de los pasos es parte del comportamiento (ver RH-01): no reordenar.
    /// </summary>
    public sealed class CalculadoraPrimaAutos
    {
        public PrimaCalculada Calcular(SolicitudCotizacion solicitud)
        {
            if (solicitud == null)
            {
                throw new ArgumentNullException(nameof(solicitud));
            }

            decimal prima = solicitud.SumaAsegurada * TarifarioAutos.TasaBase(solicitud.TipoVehiculo);
            prima *= FactorEdad(solicitud.EdadConductor);
            prima *= FactorAntiguedad(solicitud.AntiguedadVehiculo);
            prima *= TarifarioAutos.FactorZona(solicitud.Zona);

            // RH-01: la mínima se aplica aquí, ANTES del ajuste por siniestralidad.
            prima = Math.Max(prima, TarifarioAutos.PrimaMinima);
            prima *= FactorSiniestralidad(solicitud.SiniestrosUltimos3Anios);

            prima = Math.Round(prima, 2, ReglasHeredadas.Redondeo);
            decimal iva = Math.Round(prima * TarifarioAutos.TasaIva, 2, ReglasHeredadas.Redondeo);

            return new PrimaCalculada(prima, iva);
        }

        private static decimal FactorEdad(int edad)
        {
            if (edad < 25) return TarifarioAutos.RecargoConductorJoven;
            if (edad > 65) return TarifarioAutos.RecargoConductorMayor;
            return 1m;
        }

        private static decimal FactorAntiguedad(int antiguedad)
            => ReglasHeredadas.AplicaRecargoPorAntiguedad(antiguedad) ? TarifarioAutos.RecargoAntiguedad : 1m;

        private static decimal FactorSiniestralidad(int siniestros)
        {
            if (siniestros == 0) return TarifarioAutos.BonificacionSinSiniestros;
            if (siniestros >= 3) return TarifarioAutos.RecargoAltaSiniestralidad;
            return 1m;
        }
    }
}
