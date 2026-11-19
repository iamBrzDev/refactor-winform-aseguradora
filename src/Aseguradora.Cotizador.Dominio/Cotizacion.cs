using System;

namespace Aseguradora.Cotizador.Dominio
{
    /// <summary>Entidad: una cotización emitida, con identidad propia.</summary>
    public sealed class Cotizacion
    {
        public Cotizacion(Guid id, SolicitudCotizacion solicitud, PrimaCalculada prima)
        {
            if (id == Guid.Empty)
            {
                throw new ExcepcionDominio("La cotización requiere un identificador.");
            }

            Id = id;
            Solicitud = solicitud ?? throw new ArgumentNullException(nameof(solicitud));
            Prima = prima ?? throw new ArgumentNullException(nameof(prima));
        }

        public Guid Id { get; }
        public SolicitudCotizacion Solicitud { get; }
        public PrimaCalculada Prima { get; }
    }
}
