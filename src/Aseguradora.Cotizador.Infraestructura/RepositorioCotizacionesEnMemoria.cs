using System;
using System.Collections.Concurrent;
using Aseguradora.Cotizador.Aplicacion.Puertos;
using Aseguradora.Cotizador.Dominio;

namespace Aseguradora.Cotizador.Infraestructura
{
    /// <summary>
    /// Adaptador en memoria para la demo. En producción se reemplaza por un adaptador SQL
    /// que escribe en la misma tabla Cotizacion que hoy usa el formulario.
    /// </summary>
    public sealed class RepositorioCotizacionesEnMemoria : IRepositorioCotizaciones
    {
        private readonly ConcurrentDictionary<Guid, Cotizacion> _cotizaciones = new ConcurrentDictionary<Guid, Cotizacion>();

        public void Guardar(Cotizacion cotizacion)
        {
            if (cotizacion == null)
            {
                throw new ArgumentNullException(nameof(cotizacion));
            }
            _cotizaciones[cotizacion.Id] = cotizacion;
        }

        public Cotizacion ObtenerPorId(Guid id)
            => _cotizaciones.TryGetValue(id, out var cotizacion) ? cotizacion : null;
    }
}
