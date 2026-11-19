using System;
using Aseguradora.Cotizador.Dominio;

namespace Aseguradora.Cotizador.Aplicacion.Cotizaciones
{
    public sealed class CotizarPrimaComando
    {
        public CotizarPrimaComando(decimal sumaAsegurada, TipoVehiculo tipoVehiculo, int anioVehiculo,
            DateTime fechaNacimientoConductor, int siniestrosUltimos3Anios, Zona zona)
        {
            SumaAsegurada = sumaAsegurada;
            TipoVehiculo = tipoVehiculo;
            AnioVehiculo = anioVehiculo;
            FechaNacimientoConductor = fechaNacimientoConductor;
            SiniestrosUltimos3Anios = siniestrosUltimos3Anios;
            Zona = zona;
        }

        public decimal SumaAsegurada { get; }
        public TipoVehiculo TipoVehiculo { get; }
        public int AnioVehiculo { get; }
        public DateTime FechaNacimientoConductor { get; }
        public int SiniestrosUltimos3Anios { get; }
        public Zona Zona { get; }
    }
}
