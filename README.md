# Refactorizar sin apagar el negocio
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

> Las reglas de arriba describen lo que el formulario hace hoy. La demo las fija con pruebas de caracterización
> **antes** de mover una sola línea; lo que esas pruebas revelen se discute en vivo.

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

## 4. Estructura (punto de partida)

```
├── Aseguradora.Cotizador.slnx
├── global.json · Directory.Build.targets · .gitignore
└── src/Legacy/
    ├── Aseguradora.Tarifas/                 (VB.NET, tabla de tarifas de 2009)
    └── Aseguradora.Cotizador.WinForms/      (C#, net48: FrmCotizacionAuto)
```

El resto de capas (Dominio, Aplicación, Infraestructura, API, pruebas, CI, documentación) aparece paso a paso en los tags de la sección 5.

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

La demo en vivo arranca en `paso-1-claude-md` (ya trae `CLAUDE.md` y permisos):

```bash
git checkout -b demo paso-1-claude-md
claude
```

Los prompts exactos, los riesgos y el plan de reversa están en la rama `solucion`. Si en algún paso el resultado se desvía, compáralo con el tag correspondiente (`git diff paso-N-... -- ruta`).

### Ejecutar el formulario (Windows)

```powershell
copy src\Legacy\Aseguradora.Cotizador.WinForms\secrets.config.example src\Legacy\Aseguradora.Cotizador.WinForms\secrets.config
dotnet run --project src\Legacy\Aseguradora.Cotizador.WinForms
```

## 7. Seguridad del material

- Sin claves de API, tokens ni cadenas de conexión reales. `secrets.config` está en `.gitignore` y en la lista `deny` de Claude Code.
- Ningún fragmento proviene de código de empleadores o clientes.
