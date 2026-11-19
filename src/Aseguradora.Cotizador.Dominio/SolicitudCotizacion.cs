using System;

namespace Aseguradora.Cotizador.Dominio
{
    /// <summary>
    /// Datos de entrada de una cotización. Inmutable.
    /// Solo replica las validaciones que ya hacía el formulario (suma &gt; 0, siniestros &gt;= 0):
    /// agregar validaciones nuevas cambiaría el comportamiento y es decisión de negocio.
    /// </summary>
    public sealed class SolicitudCotizacion
    {
        public SolicitudCotizacion(
            decimal sumaAsegurada,
            TipoVehiculo tipoVehiculo,
            int anioVehiculo,
            DateTime fechaNacimientoConductor,
            int siniestrosUltimos3Anios,
            Zona zona,
            DateTime fechaCotizacion)
        {
            if (sumaAsegurada <= 0)
            {
                throw new ExcepcionDominio("La suma asegurada debe ser mayor que cero.");
            }

            if (siniestrosUltimos3Anios < 0)
            {
                throw new ExcepcionDominio("La cantidad de siniestros no puede ser negativa.");
            }

            SumaAsegurada = sumaAsegurada;
            TipoVehiculo = tipoVehiculo;
            AnioVehiculo = anioVehiculo;
            FechaNacimientoConductor = fechaNacimientoConductor;
            SiniestrosUltimos3Anios = siniestrosUltimos3Anios;
            Zona = zona;
            FechaCotizacion = fechaCotizacion;
        }

        public decimal SumaAsegurada { get; }
        public TipoVehiculo TipoVehiculo { get; }
        public int AnioVehiculo { get; }
        public DateTime FechaNacimientoConductor { get; }
        public int SiniestrosUltimos3Anios { get; }
        public Zona Zona { get; }
        public DateTime FechaCotizacion { get; }

        public int EdadConductor => ReglasHeredadas.EdadPorAnioCalendario(FechaNacimientoConductor, FechaCotizacion);

        public int AntiguedadVehiculo => FechaCotizacion.Year - AnioVehiculo;
    }
}
