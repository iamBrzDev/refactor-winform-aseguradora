using System;

namespace Aseguradora.Cotizador.Aplicacion.Puertos
{
    /// <summary>Puerto de tiempo: la fecha de cotización entra por aquí y el núcleo nunca consulta el reloj del sistema.</summary>
    public interface IReloj
    {
        DateTime Ahora { get; }
    }
}
