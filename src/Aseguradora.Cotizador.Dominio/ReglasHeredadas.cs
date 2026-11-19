using System;

namespace Aseguradora.Cotizador.Dominio
{
    /// <summary>
    /// Comportamientos que el negocio aceptó como normales. NO son bugs a corregir dentro del refactor.
    /// Cada uno está fijado por una prueba con [Trait("Rareza", "RH-0N")].
    /// Cambiar cualquiera requiere ticket de negocio y actualizar su prueba de caracterización.
    /// </summary>
    public static class ReglasHeredadas
    {
        /// <summary>
        /// RH-01: la prima mínima se aplica ANTES del ajuste por siniestralidad,
        /// por eso la prima final puede quedar por debajo de <see cref="TarifarioAutos.PrimaMinima"/>.
        /// (El orden está implementado en <see cref="CalculadoraPrimaAutos.Calcular"/>.)
        /// </summary>
        public const bool PrimaMinimaAntesDeSiniestralidad = true;

        /// <summary>RH-02: la edad se calcula solo con el año calendario, sin considerar mes ni día.</summary>
        public static int EdadPorAnioCalendario(DateTime nacimiento, DateTime fechaCotizacion)
            => fechaCotizacion.Year - nacimiento.Year;

        /// <summary>RH-03: el recargo aplica con antigüedad ESTRICTAMENTE mayor a 10 (el manual dice "10 o más").</summary>
        public static bool AplicaRecargoPorAntiguedad(int antiguedad) => antiguedad > 10;

        /// <summary>RH-04: redondeo bancario (MidpointRounding.ToEven), el predeterminado de Math.Round en el legado.</summary>
        public const MidpointRounding Redondeo = MidpointRounding.ToEven;
    }
}
