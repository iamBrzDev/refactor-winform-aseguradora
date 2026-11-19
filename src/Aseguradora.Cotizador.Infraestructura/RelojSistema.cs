using System;
using Aseguradora.Cotizador.Aplicacion.Puertos;

namespace Aseguradora.Cotizador.Infraestructura
{
    /// <summary>Único punto del sistema nuevo que lee el reloj de la máquina.</summary>
    public sealed class RelojSistema : IReloj
    {
        public DateTime Ahora => DateTime.Now;
    }
}
