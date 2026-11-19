using System.Net;
using System.Net.Http.Json;
using Aseguradora.Cotizador.Aplicacion.Puertos;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Aseguradora.Cotizador.Api.Tests;

public sealed class CotizacionesApiTests : IClassFixture<WebApplicationFactory<Program>>
{
    private static readonly DateTime Hoy = new(2026, 11, 17);
    private readonly HttpClient _cliente;

    public CotizacionesApiTests(WebApplicationFactory<Program> fabrica)
    {
        _cliente = fabrica.WithWebHostBuilder(b => b.ConfigureTestServices(servicios =>
        {
            servicios.RemoveAll<IReloj>();
            servicios.AddSingleton<IReloj>(new RelojFijo(Hoy));
        })).CreateClient();
    }

    [Fact]
    public async Task Post_caso_tipico_devuelve_201_con_la_prima_caracterizada()
    {
        var respuesta = await _cliente.PostAsJsonAsync("/api/v1/cotizaciones",
            new { sumaAsegurada = 15000, tipoVehiculo = "SEDAN", anioVehiculo = 2020, fechaNacimientoConductor = "1985-03-10", siniestrosUltimos3Anios = 1, zona = "URBANA" });

        Assert.Equal(HttpStatusCode.Created, respuesta.StatusCode);
        var cotizacion = await respuesta.Content.ReadFromJsonAsync<CotizacionRespuesta>();
        Assert.NotNull(cotizacion);
        Assert.Equal(577.50m, cotizacion.PrimaNeta);
        Assert.Equal(75.08m, cotizacion.Iva);
        Assert.Equal(652.58m, cotizacion.Total);
        Assert.Equal($"/api/v1/cotizaciones/{cotizacion.Id}", respuesta.Headers.Location?.OriginalString);
    }

    [Fact]
    [Trait("Rareza", "RH-01")]
    public async Task Post_respeta_la_rareza_de_prima_minima_antes_del_descuento()
    {
        var respuesta = await _cliente.PostAsJsonAsync("/api/v1/cotizaciones",
            new { sumaAsegurada = 2000, tipoVehiculo = "MOTO", anioVehiculo = 2022, fechaNacimientoConductor = "1980-05-01", siniestrosUltimos3Anios = 0, zona = "RURAL" });

        var cotizacion = await respuesta.Content.ReadFromJsonAsync<CotizacionRespuesta>();
        Assert.Equal(135.00m, cotizacion!.PrimaNeta);
        Assert.Equal(152.55m, cotizacion.Total);
    }

    [Fact]
    public async Task Post_con_codigo_desconocido_devuelve_400_con_problem_details()
    {
        var respuesta = await _cliente.PostAsJsonAsync("/api/v1/cotizaciones",
            new { sumaAsegurada = 2000, tipoVehiculo = "TRACTOR", anioVehiculo = 2022, fechaNacimientoConductor = "1980-05-01", siniestrosUltimos3Anios = 0, zona = "RURAL" });

        Assert.Equal(HttpStatusCode.BadRequest, respuesta.StatusCode);
        Assert.Equal("application/problem+json", respuesta.Content.Headers.ContentType?.MediaType);
    }

    [Fact]
    public async Task Post_con_suma_cero_devuelve_400_por_regla_de_dominio()
    {
        var respuesta = await _cliente.PostAsJsonAsync("/api/v1/cotizaciones",
            new { sumaAsegurada = 0, tipoVehiculo = "SEDAN", anioVehiculo = 2022, fechaNacimientoConductor = "1980-05-01", siniestrosUltimos3Anios = 0, zona = "URBANA" });

        Assert.Equal(HttpStatusCode.BadRequest, respuesta.StatusCode);
    }

    [Fact]
    public async Task Post_sin_fecha_de_nacimiento_ni_anio_devuelve_400()
    {
        var respuesta = await _cliente.PostAsJsonAsync("/api/v1/cotizaciones",
            new { sumaAsegurada = 2000, tipoVehiculo = "SEDAN", siniestrosUltimos3Anios = 0, zona = "URBANA" });

        Assert.Equal(HttpStatusCode.BadRequest, respuesta.StatusCode);
        var cuerpo = await respuesta.Content.ReadAsStringAsync();
        Assert.Contains("fechaNacimientoConductor", cuerpo);
        Assert.Contains("anioVehiculo", cuerpo);
    }

    [Fact]
    public async Task Post_con_fecha_y_anio_en_valor_por_defecto_devuelve_400()
    {
        var respuesta = await _cliente.PostAsJsonAsync("/api/v1/cotizaciones",
            new { sumaAsegurada = 2000, tipoVehiculo = "SEDAN", anioVehiculo = 0, fechaNacimientoConductor = "0001-01-01", siniestrosUltimos3Anios = 0, zona = "URBANA" });

        Assert.Equal(HttpStatusCode.BadRequest, respuesta.StatusCode);
    }

    [Fact]
    public async Task Get_de_una_cotizacion_registrada_la_devuelve_y_una_inexistente_da_404()
    {
        var creada = await _cliente.PostAsJsonAsync("/api/v1/cotizaciones",
            new { sumaAsegurada = 3930, tipoVehiculo = "SEDAN", anioVehiculo = 2023, fechaNacimientoConductor = "1985-03-10", siniestrosUltimos3Anios = 1, zona = "URBANA" });
        var cotizacion = await creada.Content.ReadFromJsonAsync<CotizacionRespuesta>();

        var consultada = await _cliente.GetFromJsonAsync<CotizacionRespuesta>($"/api/v1/cotizaciones/{cotizacion!.Id}");
        Assert.Equal(151.30m, consultada!.PrimaNeta);

        var inexistente = await _cliente.GetAsync($"/api/v1/cotizaciones/{Guid.NewGuid()}");
        Assert.Equal(HttpStatusCode.NotFound, inexistente.StatusCode);
    }

    public sealed record CotizacionRespuesta(Guid Id, DateTime FechaCotizacion, decimal PrimaNeta, decimal Iva, decimal Total);

    private sealed class RelojFijo(DateTime ahora) : IReloj
    {
        public DateTime Ahora { get; } = ahora;
    }
}
