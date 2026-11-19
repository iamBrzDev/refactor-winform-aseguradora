using System;

namespace Aseguradora.Cotizador.Dominio
{
    /// <summary>Una regla de negocio impide construir el objeto solicitado.</summary>
    public sealed class ExcepcionDominio : Exception
    {
        public ExcepcionDominio(string mensaje) : base(mensaje)
        {
        }
    }
}
