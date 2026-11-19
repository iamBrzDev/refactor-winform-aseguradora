namespace Aseguradora.Cotizador.Caracterizacion.Tests;

public sealed record Escenario(decimal Suma, string Tipo, int AnioVehiculo, DateTime Nacimiento, int Siniestros, string Zona);

/// <summary>
/// Producto cartesiano de valores elegidos en los límites de cada regla
/// (edad 17/24/25/41/65/66, antigüedad 0/10/11/28, siniestros 0/1/2/3/7, prima bajo y sobre la mínima,
/// y una suma que dispara el redondeo bancario).
/// </summary>
public static class Escenarios
{
    private static readonly decimal[] Sumas = { 1500m, 2000m, 3930m, 9999.99m, 15000m, 48250.50m };
    private static readonly string[] Tipos = { "SEDAN", "PICKUP", "MOTO", "MICROBUS" };
    private static readonly int[] Anios = { 2026, 2016, 2015, 1998 };
    private static readonly DateTime[] Nacimientos =
    {
        new(2008, 12, 31), new(2002, 1, 5), new(2001, 12, 20),
        new(1985, 3, 10), new(1961, 6, 15), new(1960, 12, 31),
    };
    private static readonly int[] Siniestros = { 0, 1, 2, 3, 7 };
    private static readonly string[] Zonas = { "URBANA", "RURAL" };

    public static IEnumerable<Escenario> Todos()
    {
        foreach (var suma in Sumas)
        foreach (var tipo in Tipos)
        foreach (var anio in Anios)
        foreach (var nacimiento in Nacimientos)
        foreach (var siniestros in Siniestros)
        foreach (var zona in Zonas)
            yield return new Escenario(suma, tipo, anio, nacimiento, siniestros, zona);
    }

    /// <summary>Muestra reducida para pruebas parametrizadas visibles en el explorador de pruebas.</summary>
    public static IEnumerable<object[]> Muestra() =>
        Todos().Where((_, i) => i % 97 == 0).Select(e => new object[] { e });
}
