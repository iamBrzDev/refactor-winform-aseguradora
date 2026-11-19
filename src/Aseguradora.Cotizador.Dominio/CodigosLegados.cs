namespace Aseguradora.Cotizador.Dominio
{
    /// <summary>
    /// Traduce los códigos que usa el sistema heredado (y su base de datos) al lenguaje del dominio.
    /// Diferencia consciente con el legado: un código desconocido se rechaza en lugar de caer en la
    /// "tasa general". El formulario solo ofrece los cuatro códigos válidos, así que no cambia nada observable.
    /// <para>
    /// Normalización: <c>Trim().ToUpperInvariant()</c> ignora mayúsculas/minúsculas y espacios en los extremos
    /// ("sedan " se lee como SEDAN). El <c>Select Case</c> del módulo VB.NET legado (<c>ModTarifas</c>) es sensible a
    /// ambos: allí "sedan" o "SEDAN " caen en <c>Case Else</c> ("tasa general" / factor 1). También es una diferencia
    /// consciente: no se observa desde el formulario, pero sí desde clientes que llamen a la API con otro formato.
    /// </para>
    /// </summary>
    public static class CodigosLegados
    {
        public static bool TryTipoVehiculo(string codigo, out TipoVehiculo tipo)
        {
            switch ((codigo ?? string.Empty).Trim().ToUpperInvariant())
            {
                case "SEDAN": tipo = TipoVehiculo.Sedan; return true;
                case "PICKUP": tipo = TipoVehiculo.Pickup; return true;
                case "MOTO": tipo = TipoVehiculo.Moto; return true;
                case "MICROBUS": tipo = TipoVehiculo.Microbus; return true;
                default: tipo = default; return false;
            }
        }

        public static bool TryZona(string codigo, out Zona zona)
        {
            switch ((codigo ?? string.Empty).Trim().ToUpperInvariant())
            {
                case "URBANA": zona = Zona.Urbana; return true;
                case "RURAL": zona = Zona.Rural; return true;
                default: zona = default; return false;
            }
        }

        public static TipoVehiculo ParseTipoVehiculo(string codigo)
        {
            if (!TryTipoVehiculo(codigo, out var tipo))
            {
                throw new ExcepcionDominio($"Tipo de vehículo desconocido: '{codigo}'.");
            }
            return tipo;
        }

        public static Zona ParseZona(string codigo)
        {
            if (!TryZona(codigo, out var zona))
            {
                throw new ExcepcionDominio($"Zona desconocida: '{codigo}'.");
            }
            return zona;
        }
    }
}
