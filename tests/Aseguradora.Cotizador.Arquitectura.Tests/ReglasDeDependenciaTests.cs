using System.Reflection;
using Aseguradora.Cotizador.Aplicacion.Cotizaciones;
using Aseguradora.Cotizador.Dominio;
using Aseguradora.Cotizador.Infraestructura;
using NetArchTest.Rules;

namespace Aseguradora.Cotizador.Arquitectura.Tests;

/// <summary>
/// Reglas estructurales de arquitectura limpia.
/// - Referencias entre proyectos: reflexión sobre <c>GetReferencedAssemblies()</c>.
/// - Uso de frameworks y capas externas: NetArchTest inspecciona los tipos referenciados por el IL. Un ensamblado
///   netstandard2.0 solo referencia a "netstandard", así que mirar ensamblados referenciados NO detecta un uso de
///   System.Data o System.Configuration: hay que mirar namespaces.
/// Lo semántico (fecha del sistema, literales, secretos) lo revisa el subagente guardian-arquitectura.
/// </summary>
public sealed class ReglasDeDependenciaTests
{
    private static readonly Assembly Dominio = typeof(CalculadoraPrimaAutos).Assembly;
    private static readonly Assembly Aplicacion = typeof(CotizarPrima).Assembly;
    private static readonly Assembly Infraestructura = typeof(RelojSistema).Assembly;

    private static readonly string[] FrameworksYBordesProhibidosEnElNucleo =
    {
        "System.Data", "System.Windows.Forms", "System.Configuration", "Microsoft.AspNetCore",
        "Microsoft.EntityFrameworkCore", "Microsoft.VisualBasic",
        "Aseguradora.Tarifas", "Aseguradora.Cotizador.WinForms", "Aseguradora.Cotizador.Api",
    };

    [Fact]
    public void Dominio_no_depende_de_ningun_otro_ensamblado_del_sistema()
    {
        Assert.Empty(ReferenciasPropias(Dominio));
    }

    [Fact]
    public void Aplicacion_solo_depende_de_Dominio()
    {
        Assert.Equal(new[] { "Aseguradora.Cotizador.Dominio" }, ReferenciasPropias(Aplicacion));
    }

    [Fact]
    public void Infraestructura_no_depende_de_los_bordes()
    {
        var referencias = ReferenciasPropias(Infraestructura);
        Assert.DoesNotContain("Aseguradora.Cotizador.Api", referencias);
        Assert.DoesNotContain("Cotizador", referencias);
    }

    [Fact]
    public void Dominio_no_usa_frameworks_de_UI_datos_web_ni_codigo_heredado()
    {
        AssertSinDependencias(Dominio, FrameworksYBordesProhibidosEnElNucleo);
    }

    [Fact]
    public void Aplicacion_no_usa_frameworks_de_UI_datos_web_ni_codigo_heredado()
    {
        AssertSinDependencias(Aplicacion, FrameworksYBordesProhibidosEnElNucleo);
    }

    [Fact]
    public void Dominio_no_usa_Aplicacion_ni_Infraestructura()
    {
        AssertSinDependencias(Dominio,
            "Aseguradora.Cotizador.Aplicacion", "Aseguradora.Cotizador.Infraestructura");
    }

    [Fact]
    public void Aplicacion_no_usa_Infraestructura()
    {
        AssertSinDependencias(Aplicacion, "Aseguradora.Cotizador.Infraestructura");
    }

    [Fact]
    public void Infraestructura_no_usa_los_bordes_ni_UI()
    {
        AssertSinDependencias(Infraestructura,
            "System.Windows.Forms", "Microsoft.AspNetCore", "Aseguradora.Tarifas",
            "Aseguradora.Cotizador.WinForms", "Aseguradora.Cotizador.Api");
    }

    /// <summary>Control de la propia prueba: si el detector dejara de ver System.Data, esta prueba fallaría.</summary>
    [Fact]
    public void El_detector_si_encuentra_un_uso_de_System_Data()
    {
        var resultado = Types.InAssembly(typeof(SentinelaConDependenciaProhibida).Assembly)
            .That().HaveName(nameof(SentinelaConDependenciaProhibida))
            .ShouldNot().HaveDependencyOn("System.Data")
            .GetResult();

        Assert.False(resultado.IsSuccessful);
    }

    [Fact]
    public void Tipos_publicos_del_dominio_son_sellados_estaticos_o_abstractos()
    {
        var abiertos = Dominio.GetExportedTypes()
            .Where(t => t.IsClass && !t.IsSealed && !t.IsAbstract)
            .Select(t => t.FullName)
            .ToList();

        Assert.Empty(abiertos);
    }

    [Fact]
    public void El_dominio_no_expone_setters_publicos()
    {
        // DeclaredOnly: solo las propiedades que el dominio declara (no las heredadas de System.Exception).
        var conSetter = Dominio.GetExportedTypes()
            .SelectMany(t => t.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly)
                .Where(p => p.SetMethod?.IsPublic == true)
                .Select(p => $"{t.Name}.{p.Name}"))
            .ToList();

        Assert.Empty(conSetter);
    }

    private static void AssertSinDependencias(Assembly ensamblado, params string[] prohibidas)
    {
        var resultado = Types.InAssembly(ensamblado)
            .ShouldNot().HaveDependencyOnAny(prohibidas)
            .GetResult();

        Assert.True(resultado.IsSuccessful,
            "Dependencias prohibidas en: " + string.Join(", ", resultado.FailingTypeNames ?? Array.Empty<string>()));
    }

    private static string[] ReferenciasPropias(Assembly ensamblado) =>
        ensamblado.GetReferencedAssemblies()
            .Select(a => a.Name!)
            .Where(n => n.StartsWith("Aseguradora.") || n == "Cotizador")
            .OrderBy(n => n)
            .ToArray();

    /// <summary>Tipo de prueba que usa System.Data a propósito (ver El_detector_si_encuentra_un_uso_de_System_Data).</summary>
    internal sealed class SentinelaConDependenciaProhibida
    {
        public object Crear() => new System.Data.DataTable();
    }
}
