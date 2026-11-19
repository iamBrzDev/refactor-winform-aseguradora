using System;
using Aseguradora.Cotizador.Dominio;

namespace Aseguradora.Cotizador.Aplicacion.Cotizaciones
{
    /// <summary>Lo que el núcleo entrega hacia los bordes (API, WinForms). Sin tipos de dominio expuestos.</summary>
    public sealed class CotizacionDto
    {
        public Guid Id { get; set; }
        public DateTime FechaCotizacion { get; set; }
        public decimal PrimaNeta { get; set; }
        public decimal Iva { get; set; }
        public decimal Total { get; set; }

        public static CotizacionDto Desde(Cotizacion cotizacion) => new CotizacionDto
        {
            Id = cotizacion.Id,
            FechaCotizacion = cotizacion.Solicitud.FechaCotizacion,
            PrimaNeta = cotizacion.Prima.PrimaNeta,
            Iva = cotizacion.Prima.Iva,
            Total = cotizacion.Prima.Total,
        };
    }
}
