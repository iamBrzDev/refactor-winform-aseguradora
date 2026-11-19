using System;
using Aseguradora.Cotizador.Dominio;

namespace Aseguradora.Cotizador.Aplicacion.Puertos
{
    public interface IRepositorioCotizaciones
    {
        void Guardar(Cotizacion cotizacion);

        /// <summary>Devuelve null si no existe.</summary>
        Cotizacion ObtenerPorId(Guid id);
    }
}
