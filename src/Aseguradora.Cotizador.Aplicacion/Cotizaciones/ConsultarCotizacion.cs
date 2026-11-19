using System;
using Aseguradora.Cotizador.Aplicacion.Puertos;

namespace Aseguradora.Cotizador.Aplicacion.Cotizaciones
{
    public sealed class ConsultarCotizacion
    {
        private readonly IRepositorioCotizaciones _repositorio;

        public ConsultarCotizacion(IRepositorioCotizaciones repositorio)
        {
            _repositorio = repositorio ?? throw new ArgumentNullException(nameof(repositorio));
        }

        /// <summary>Devuelve null si la cotización no existe.</summary>
        public CotizacionDto Ejecutar(Guid id)
        {
            var cotizacion = _repositorio.ObtenerPorId(id);
            return cotizacion == null ? null : CotizacionDto.Desde(cotizacion);
        }
    }
}
