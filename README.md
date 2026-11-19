# Refactorizar sin apagar el negocio

[![ci](https://github.com/iamBrzDev/refactor-winform-aseguradora/actions/workflows/ci.yml/badge.svg)](https://github.com/iamBrzDev/refactor-winform-aseguradora/actions/workflows/ci.yml)
### De un formulario Windows Forms a arquitectura limpia con Claude Code

Material de la clase demo para el bootcamp **«Arquitectura Limpia y Refactorización Asistida con Claude Code»**.
Todo el código, los datos y las reglas de negocio son **ficticios**.

---

## 1. Objetivo

Mostrar, con un caso del sector asegurador, un método repetible para sacar lógica de negocio de un
formulario heredado **sin cambiar su comportamiento y sin detener la operación**:

1. **Controlar al asistente**: `CLAUDE.md` con las reglas del equipo y permisos que excluyen secretos.
2. **Fijar el comportamiento actual**: pruebas de caracterización con xUnit (incluidas las rarezas que el negocio ya aceptó).
3. **Extraer** la lógica a un dominio con arquitectura limpia y **exponerla** como Minimal API.
4. **Verificar** con las mismas pruebas en verde antes y después, más un subagente que vigila las reglas de arquitectura.
5. **Convivir y revertir**: el formulario elige motor (`Legacy` / `Sombra` / `Nuevo`) desde configuración, sin redeploy.

## 2. El caso: `FrmCotizacionAuto`

Cotizador de seguro de auto en ventanilla, en producción desde 2011. Toda la lógica vive en `btnCalcular_Click`
y la tabla de tarifas está en un módulo **VB.NET** de 2009.

| Paso | Regla |
|---|---|
| 1 | Prima base = suma asegurada × tasa (SEDAN 3.5 %, PICKUP 4.2 %, MOTO 6 %, otro 5 %) |
| 2 | Edad del conductor < 25 → +25 %; > 65 → +15 % |
| 3 | Antigüedad del vehículo > 10 años → +20 % |
| 4 | Zona URBANA ×1.10, RURAL ×0.95 |
| 5 | Prima mínima US$150 |
| 6 | 0 siniestros → −10 %; 3 o más → +30 % |
| 7 | Redondeo a 2 decimales, IVA 13 % redondeado aparte, total = prima + IVA |

### Rarezas que el negocio ya aceptó (y que el refactor **no** corrige)

| ID | Comportamiento | Ejemplo |
|---|---|---|
| RH-01 | **La prima mínima se aplica antes del descuento por cero siniestros**, así que la prima final puede quedar por debajo de US$150. Comercial lo vende como "descuento de buen cliente sobre la mínima". | Moto US$2,000, rural, 0 siniestros → **US$135.00** |
| RH-02 | La edad se calcula solo con el año (`hoy.Year - nacimiento.Year`): alguien de 24 años que cumple en diciembre se cotiza como de 25. | Nació 20/12/2001, cotiza 17/11/2026 → sin recargo |
| RH-03 | El manual dice "10 años o más", el código dice `> 10`: un vehículo de exactamente 10 años no lleva recargo. | Año 2016 en 2026 → sin recargo |
| RH-04 | `Math.Round` usa redondeo bancario (`ToEven`), no "hacia arriba". | 151.305 → **151.30** |

> Corregir cualquiera de estas reglas es una **decisión de negocio** (ticket + aprobación), no parte del refactor.

## 3. Requisitos del entorno

| Herramienta | Versión | Para qué |
|---|---|---|
| Visual Studio 2026 (18.0+) **o** `dotnet` CLI | `net10.0` | Abrir la solución (`.slnx`) y compilar/probar Dominio, API y pruebas |
| Windows 10/11 | — | Solo para compilar y ejecutar el formulario (.NET Framework 4.8) |
| .NET Framework 4.8 Developer Pack | 4.8 | Proyectos heredados (opcional: `Directory.Build.targets` trae los ensamblados de referencia por NuGet) |
| .NET SDK | 10.0.100+ | Dominio, API y pruebas (ver `global.json`) |
| Claude Code | última | Instalador nativo — Windows (PowerShell): `irm https://claude.ai/install.ps1 \| iex`; macOS/Linux: `curl -fsSL https://claude.ai/install.sh \| bash`. Luego `claude` en la raíz del repo |
| Git | 2.40+ | Navegar por los tags de cada paso |

> Las pruebas y la API corren también en Linux/macOS. Solo el formulario necesita Windows.

## 4. Estructura

```
├── CLAUDE.md                         ← reglas del equipo para Claude Code
├── .claude/
│   ├── settings.json                 ← permisos: allow / ask / deny (secretos excluidos)
│   ├── agents/guardian-arquitectura.md   ← subagente revisor (no edita; Bash solo lectura y pruebas)
│   └── skills/caracterizar-legacy/SKILL.md ← skill para generar pruebas de caracterización
├── src/
│   ├── Legacy/
│   │   ├── Aseguradora.Tarifas/                 (VB.NET, net48 + netstandard2.0)
│   │   └── Aseguradora.Cotizador.WinForms/      (C#, net48)
│   ├── Aseguradora.Cotizador.Dominio/           (netstandard2.0, sin dependencias)
│   ├── Aseguradora.Cotizador.Aplicacion/        (netstandard2.0, casos de uso y puertos)
│   ├── Aseguradora.Cotizador.Infraestructura/   (netstandard2.0, adaptadores)
│   └── Aseguradora.Cotizador.Api/               (net10.0, Minimal API)
├── tests/
│   ├── Aseguradora.Cotizador.Caracterizacion.Tests/  ← mismas pruebas contra Legacy y Dominio
│   ├── Aseguradora.Cotizador.Arquitectura.Tests/     ← reglas de dependencia entre capas (reflexión + NetArchTest)
│   └── Aseguradora.Cotizador.Api.Tests/              ← pruebas de integración de la API
├── .github/workflows/ci.yml          ← pruebas en Linux + build de la solución (.slnx) en Windows
└── docs/
    ├── guia-demo.md                  ← prompts exactos de la demo, paso por paso
    └── riesgos-y-reversa.md          ← gobierno de información y plan de reversa
```

¿Por qué el dominio es `netstandard2.0`? Porque así **lo consumen a la vez** el WinForms en .NET Framework 4.8
y la API en .NET 10. Es lo que permite el patrón *strangler*: el formulario sigue vivo mientras el núcleo migra.

## 5. Ramas y tags

| Rama / tag | Contenido |
|---|---|
| `inicio` · `paso-0-inicio` | Solo el formulario heredado (sin `CLAUDE.md` ni permisos). |
| `paso-1-claude-md` | `CLAUDE.md` + `.claude/settings.json`. **Punto de partida de la demo en vivo.** |
| `paso-2-costura` | Extracción mecánica del cálculo a `CotizadorPrimaLegacy` (único cambio: `DateTime.Now` → parámetro `hoy`) |
| `paso-3-caracterizacion` | Skill `caracterizar-legacy` + pruebas de caracterización en verde contra el código heredado |
| `paso-4-dominio` | Dominio + Aplicación + Infraestructura; las mismas pruebas en verde contra el dominio y prueba de paridad |
| `paso-5-minimal-api` | Minimal API + pruebas de integración + archivo `.http` |
| `paso-6-guardian-arquitectura` | Subagente `guardian-arquitectura` + pruebas de arquitectura |
| `paso-7-strangler` | El formulario elige motor `Legacy` / `Sombra` / `Nuevo` desde configuración |
| `solucion` (= `main`) | Estado final con documentación |

```bash
git log --oneline --decorate        # ver el paso a paso
git checkout paso-3-caracterizacion # saltar a un paso
git diff paso-1-claude-md paso-2-costura   # ver exactamente qué cambió en la costura
```

## 6. Reproducir la demo

### Opción A — Seguir la clase con Claude Code (recomendada)

```bash
git checkout -b demo paso-1-claude-md
claude
```

`paso-1-claude-md` ya trae `CLAUDE.md` y permisos (configuración del equipo: no se improvisa en vivo). Para ver el formulario sin nada de eso: `git checkout inicio`.

Sigue los prompts de [`docs/guia-demo.md`](docs/guia-demo.md). Si en algún paso el resultado se desvía,
compáralo con el tag correspondiente (`git diff paso-N-... -- ruta`).

### Opción B — Revisar la solución

```bash
git checkout solucion
dotnet test tests/Aseguradora.Cotizador.Caracterizacion.Tests
dotnet test tests/Aseguradora.Cotizador.Arquitectura.Tests
dotnet test tests/Aseguradora.Cotizador.Api.Tests
dotnet run --project src/Aseguradora.Cotizador.Api      # http://localhost:5080
```

Luego abre `src/Aseguradora.Cotizador.Api/cotizaciones.http` en Visual Studio 2026 / VS Code y ejecuta las peticiones.

### Ejecutar el formulario (Windows)

```powershell
copy src\Legacy\Aseguradora.Cotizador.WinForms\secrets.config.example src\Legacy\Aseguradora.Cotizador.WinForms\secrets.config
dotnet run --project src\Legacy\Aseguradora.Cotizador.WinForms
```

`MotorCotizacion` vale `Legacy` por defecto (`App.config`). Con el formulario **en ejecución**, el archivo que se edita para
cambiar de motor —o para revertir sin reiniciar— es `bin\Debug\net48\Cotizador.exe.config` (en producción, `Cotizador.exe.config`
junto al ejecutable), **no** `App.config`: este es solo la plantilla que se copia al compilar, no afecta al proceso que
ya está corriendo y un nuevo build lo sobrescribe. Valores: `Legacy`, `Sombra`, `Nuevo`; el cambio se toma en el **siguiente clic**.

- `Sombra`: se muestra el resultado heredado y las diferencias con el motor nuevo se escriben en `sombra.log` (junto al ejecutable).
- `Nuevo`: si el motor nuevo lanza una excepción, se registra con `Trace.TraceError` y se responde con `Legacy`.

## 7. Seguridad del material

- Sin claves de API, tokens ni cadenas de conexión reales. `secrets.config` está en `.gitignore` y en la lista `deny` de Claude Code.
- Ningún fragmento proviene de código de empleadores o clientes.
- Ver [`docs/riesgos-y-reversa.md`](docs/riesgos-y-reversa.md) para la política de uso del asistente con código propietario.
