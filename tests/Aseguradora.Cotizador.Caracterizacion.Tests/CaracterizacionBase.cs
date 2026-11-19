namespace Aseguradora.Cotizador.Caracterizacion.Tests;

/// <summary>
/// Pruebas de caracterización del cotizador de autos.
/// Registran lo que el sistema HACE hoy (no lo que "debería" hacer).
/// Se escriben una sola vez y se ejecutan contra cada implementación:
/// Legacy (antes del refactor) y Dominio (después). Ambas deben quedar en verde.
/// </summary>
public abstract class CaracterizacionBase
{
    /// <summary>Fecha de cotización fija: elimina la dependencia de DateTime.Now.</summary>
    protected static readonly DateTime Hoy = new(2026, 11, 17);

    protected abstract Resultado Cotizar(decimal suma, string tipo, int anioVehiculo,
        DateTime nacimiento, int siniestros, string zona, DateTime hoy);

    // ---------------------------------------------------------------- Casos base

    [Fact]
    public void Caso_tipico_sedan_urbano_sin_recargos()
    {
        var r = Cotizar(15000m, "SEDAN", 2020, new DateTime(1985, 3, 10), 1, "URBANA", Hoy);
        Assert.Equal(new Resultado(577.50m, 75.08m, 652.58m), r);
    }

    [Fact]
    public void Conductor_menor_de_25_paga_25_por_ciento_mas()
    {
        // 2026 - 2002 = 24 -> recargo
        var r = Cotizar(10000m, "SEDAN", 2023, new DateTime(2002, 1, 5), 1, "URBANA", Hoy);
        Assert.Equal(new Resultado(481.25m, 62.56m, 543.81m), r);
    }

    [Fact]
    public void Conductor_mayor_de_65_paga_15_por_ciento_mas()
    {
        // 2026 - 1960 = 66 -> recargo
        var r = Cotizar(12000m, "PICKUP", 2019, new DateTime(1960, 6, 15), 2, "URBANA", Hoy);
        Assert.Equal(new Resultado(637.56m, 82.88m, 720.44m), r);
    }

    [Fact]
    public void Conductor_de_exactamente_65_no_lleva_recargo()
    {
        // 2026 - 1961 = 65 -> límite, sin recargo
        var r = Cotizar(12000m, "PICKUP", 2019, new DateTime(1961, 6, 15), 2, "URBANA", Hoy);
        Assert.Equal(new Resultado(554.40m, 72.07m, 626.47m), r);
    }

    [Fact]
    public void Vehiculo_de_11_anios_lleva_recargo_de_20_por_ciento()
    {
        var r = Cotizar(20000m, "PICKUP", 2015, new DateTime(1975, 1, 1), 1, "RURAL", Hoy);
        Assert.Equal(new Resultado(957.60m, 124.49m, 1082.09m), r);
    }

    [Fact]
    public void Microbus_no_esta_en_la_tabla_y_usa_la_tasa_general_de_respaldo()
    {
        var r = Cotizar(30000m, "MICROBUS", 2021, new DateTime(1978, 8, 8), 1, "URBANA", Hoy);
        Assert.Equal(new Resultado(1650.00m, 214.50m, 1864.50m), r);
    }

    [Fact]
    public void Tres_o_mas_siniestros_recargan_30_por_ciento()
    {
        var r = Cotizar(18000m, "SEDAN", 2018, new DateTime(1990, 2, 2), 3, "RURAL", Hoy);
        Assert.Equal(new Resultado(778.05m, 101.15m, 879.20m), r);
    }

    [Fact]
    public void Prima_por_debajo_de_la_minima_se_eleva_a_150()
    {
        // 2000 x 0.06 x 0.95 = 114 -> 150 (1 siniestro: sin ajuste posterior)
        var r = Cotizar(2000m, "MOTO", 2022, new DateTime(1980, 5, 1), 1, "RURAL", Hoy);
        Assert.Equal(new Resultado(150m, 19.50m, 169.50m), r);
    }

    // ---------------------------------------------------------------- Rarezas aceptadas por el negocio

    [Fact]
    [Trait("Rareza", "RH-01")]
    public void Rareza_prima_minima_se_aplica_antes_del_descuento_y_la_prima_final_queda_debajo_de_150()
    {
        // 114 -> mínima 150 -> descuento 0 siniestros -10% -> 135.00 (por debajo de la mínima)
        var r = Cotizar(2000m, "MOTO", 2022, new DateTime(1980, 5, 1), 0, "RURAL", Hoy);
        Assert.Equal(new Resultado(135.00m, 17.55m, 152.55m), r);
    }

    [Fact]
    [Trait("Rareza", "RH-02")]
    public void Rareza_la_edad_se_calcula_solo_por_anio_calendario()
    {
        // Nació el 20/12/2001: el 17/11/2026 tiene 24 años reales, pero el sistema calcula 25 -> sin recargo.
        var r = Cotizar(10000m, "SEDAN", 2023, new DateTime(2001, 12, 20), 1, "URBANA", Hoy);
        Assert.Equal(new Resultado(385.00m, 50.05m, 435.05m), r);
    }

    [Fact]
    [Trait("Rareza", "RH-03")]
    public void Rareza_vehiculo_de_exactamente_10_anios_no_lleva_recargo()
    {
        // El manual dice "10 años o más"; el código usa "> 10".
        var r = Cotizar(20000m, "PICKUP", 2016, new DateTime(1975, 1, 1), 1, "RURAL", Hoy);
        Assert.Equal(new Resultado(798.00m, 103.74m, 901.74m), r);
    }

    [Fact]
    [Trait("Rareza", "RH-04")]
    public void Rareza_la_prima_usa_redondeo_bancario()
    {
        // 3930 x 0.035 x 1.10 = 151.305 -> ToEven = 151.30 (AwayFromZero daría 151.31)
        var r = Cotizar(3930m, "SEDAN", 2023, new DateTime(1985, 3, 10), 1, "URBANA", Hoy);
        Assert.Equal(new Resultado(151.30m, 19.67m, 170.97m), r);
    }

    // ---------------------------------------------------------------- Invariantes

    [Theory]
    [MemberData(nameof(Escenarios.Muestra), MemberType = typeof(Escenarios))]
    public void El_total_siempre_es_prima_mas_iva_y_ambos_tienen_dos_decimales(Escenario e)
    {
        var r = Cotizar(e.Suma, e.Tipo, e.AnioVehiculo, e.Nacimiento, e.Siniestros, e.Zona, Hoy);

        Assert.Equal(r.PrimaNeta + r.Iva, r.Total);
        Assert.Equal(Math.Round(r.PrimaNeta, 2), r.PrimaNeta);
        Assert.Equal(Math.Round(r.Iva, 2), r.Iva);
    }
}

/// <summary>Resultado neutral para comparar implementaciones distintas.</summary>
public sealed record Resultado(decimal PrimaNeta, decimal Iva, decimal Total);
