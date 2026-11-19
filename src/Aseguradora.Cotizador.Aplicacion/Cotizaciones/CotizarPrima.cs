using System;
using Aseguradora.Cotizador.Aplicacion.Puertos;
using Aseguradora.Cotizador.Dominio;

namespace Aseguradora.Cotizador.Aplicacion.Cotizaciones
{
    /// <summary>Caso de uso: calcular y registrar una cotización.</summary>
    public sealed class CotizarPrima
    {
        private readonly IReloj _reloj;
        private readonly IRepositorioCotizaciones _repositorio;
        private readonly CalculadoraPrimaAutos _calculadora = new CalculadoraPrimaAutos();

        public CotizarPrima(IReloj reloj, IRepositorioCotizaciones repositorio)
        {
            _reloj = reloj ?? throw new ArgumentNullException(nameof(reloj));
            _repositorio = repositorio ?? throw new ArgumentNullException(nameof(repositorio));
        }

        public CotizacionDto Ejecutar(CotizarPrimaComando comando)
        {
            if (comando == null)
            {
                throw new ArgumentNullException(nameof(comando));
            }

            var solicitud = new SolicitudCotizacion(
                comando.SumaAsegurada,
                comando.TipoVehiculo,
                comando.AnioVehiculo,
                comando.FechaNacimientoConductor,
                comando.SiniestrosUltimos3Anios,
                comando.Zona,
                _reloj.Ahora);

            var cotizacion = new Cotizacion(Guid.NewGuid(), solicitud, _calculadora.Calcular(solicitud));
            _repositorio.Guardar(cotizacion);

            return CotizacionDto.Desde(cotizacion);
        }
    }
}
