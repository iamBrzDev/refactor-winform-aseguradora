using System;

namespace Aseguradora.Cotizador.Dominio
{
    /// <summary>
    /// Parámetros de tarifa. Antes vivían en ModTarifas.vb (2009) y como números mágicos en el formulario.
    /// Es el ÚNICO lugar del dominio donde se permiten literales de tarifa.
    /// </summary>
    public static class TarifarioAutos
    {
        public const decimal PrimaMinima = 150m;
        public const decimal TasaIva = 0.13m;

        public const decimal RecargoConductorJoven = 1.25m;   // edad < 25
        public const decimal RecargoConductorMayor = 1.15m;   // edad > 65
        public const decimal RecargoAntiguedad = 1.20m;       // antigüedad > 10 (RH-03)
        public const decimal BonificacionSinSiniestros = 0.90m;
        public const decimal RecargoAltaSiniestralidad = 1.30m; // 3 o más siniestros

        public static decimal TasaBase(TipoVehiculo tipo)
        {
            switch (tipo)
            {
                case TipoVehiculo.Sedan: return 0.035m;
                case TipoVehiculo.Pickup: return 0.042m;
                case TipoVehiculo.Moto: return 0.060m;
                case TipoVehiculo.Microbus: return 0.050m; // en el legado era el "Case Else"
                default: throw new ArgumentOutOfRangeException(nameof(tipo), tipo, null);
            }
        }

        public static decimal FactorZona(Zona zona)
        {
            switch (zona)
            {
                case Zona.Urbana: return 1.10m;
                case Zona.Rural: return 0.95m;
                default: throw new ArgumentOutOfRangeException(nameof(zona), zona, null);
            }
        }
    }
}
