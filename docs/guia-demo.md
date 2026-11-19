# Guía de la demo — prompts exactos, paso por paso

Cada paso tiene: **objetivo**, **prompt** para Claude Code, **qué revisar** y **red de seguridad**
(el tag con el resultado esperado, por si la generación en vivo se desvía).

> Arranque recomendado: `git checkout -b demo paso-1-claude-md` y luego `claude` en la raíz.
> El paso 1 (CLAUDE.md y permisos) se muestra ya preparado: es configuración del equipo, no se improvisa.

---

## Preparación (antes de grabar)

Requisitos: Visual Studio 2026 (18.0+) **o** `dotnet` CLI para `net10.0`; Claude Code con el instalador nativo (`irm https://claude.ai/install.ps1 | iex`).

```powershell
git clone https://github.com/iamBrzDev/refactor-winform-aseguradora.git
cd refactor-winform-aseguradora
git checkout solucion && dotnet test tests/Aseguradora.Cotizador.Caracterizacion.Tests   # calienta la caché de NuGet
copy src\Legacy\Aseguradora.Cotizador.WinForms\secrets.config.example src\Legacy\Aseguradora.Cotizador.WinForms\secrets.config
git checkout -b demo paso-1-claude-md
claude
```

Dentro de Claude Code: `/status` para mostrar que cargó `Shared project settings`.

---

## Paso 0 — Orientación (sin tocar nada)

**Prompt**
```
Lee CLAUDE.md y src/Legacy. En 5 viñetas: qué hace FrmCotizacionAuto, qué entradas implícitas
tiene el cálculo y qué riesgos ves para refactorizarlo. No modifiques nada.
```

**Qué revisar**: que detecte `DateTime.Now`, el módulo VB, la base de datos y el orden mínima → descuento.

## Paso 1 — Demostrar el control

**Prompt**
```
Muéstrame el contenido de src/Legacy/Aseguradora.Cotizador.WinForms/secrets.config
```

**Esperado**: la lectura se rechaza por la regla `deny`. Explicar que la regla también cubre `cat`/`head`,
pero no un script que abra el archivo por su cuenta: por eso los secretos reales no viven en el árbol de trabajo
(y para aislamiento a nivel de SO existe el sandbox de Claude Code).

## Paso 2 — Costura

**Prompt** (en modo plan: `Shift+Tab` hasta *plan mode*)
```
Necesito una costura para caracterizar el cálculo de btnCalcular_Click sin abrir el formulario.
Propón la extracción mecánica mínima a una clase estática CotizadorPrimaLegacy en el mismo proyecto.
Restricciones: mover el código tal cual; el único cambio permitido es reemplazar DateTime.Now por un
parámetro "hoy". Agrega netstandard2.0 al proyecto VB para poder referenciarlo desde pruebas en .NET 10.
```

**Qué revisar**
- Claude pide permiso para editar `src/Legacy/**` (regla `ask`).
- `git diff --word-diff`: solo movimientos y el parámetro `hoy`.
- Comentar: `DateTime.Now` se leía dos veces; ahora una (carrera de medianoche del 31/12 eliminada — es un cambio, aunque inofensivo).

**Red de seguridad**: `git diff paso-1-claude-md paso-2-costura`

## Paso 3 — Caracterización con la skill

**Prompt**
```
/caracterizar-legacy CotizadorPrimaLegacy
Crea tests/Aseguradora.Cotizador.Caracterizacion.Tests (xUnit, net10.0) enlazando
CotizadorPrimaLegacy.cs como archivo, con fecha fija 2026-11-17. Usa una clase base abstracta
para poder reutilizar las pruebas contra el código nuevo. Incluye las rarezas RH-01..RH-04 del README.
```

**Qué revisar**
- Los valores esperados salen de ejecutar el legado, no de "lo que debería ser".
- La prueba RH-01 espera **135.00** (por debajo de la mínima).
- `dotnet test tests/Aseguradora.Cotizador.Caracterizacion.Tests` → verde.
- Mutación rápida en vivo: cambiar `> 10` por `>= 10` en el legado → falla RH-03 → revertir **solo ese archivo**:
  `git status --short` (debe listar únicamente `CotizadorPrimaLegacy.cs`) y luego
  `git checkout -- src/Legacy/Aseguradora.Cotizador.WinForms/CotizadorPrimaLegacy.cs`.
  Si hay otros cambios sin commit (`git log --oneline -1` no los muestra), no uses una ruta amplia: se perderían.

**Red de seguridad**: `git checkout paso-3-caracterizacion -- tests .claude/skills`

## Paso 4 — Extracción a arquitectura limpia

**Prompt** (modo plan)
```
Extrae la lógica a Dominio, Aplicacion e Infraestructura (netstandard2.0) según CLAUDE.md.
Las rarezas deben quedar explícitas en una clase ReglasHeredadas y el orden del cálculo debe preservarse.
Después crea DominioCaracterizacionTests que herede de CaracterizacionBase sin modificar ningún valor
esperado, y una prueba de paridad legado vs dominio sobre Escenarios.Todos().
```

**Qué revisar**
- El plan antes de ejecutar: capas, dependencias, puertos (`IReloj`, `IRepositorioCotizaciones`).
- Ningún cambio en `CaracterizacionBase.cs` (`git diff -- tests/**/CaracterizacionBase.cs` vacío).
- Pruebas: Legacy verde, Dominio verde, Paridad verde (5,760 escenarios).

**Red de seguridad**: `git checkout paso-4-dominio -- src tests`

## Paso 5 — Minimal API

**Prompt**
```
Expón el caso de uso CotizarPrima como Minimal API en src/Aseguradora.Cotizador.Api (net10.0):
POST /api/v1/cotizaciones y GET /api/v1/cotizaciones/{id}, errores como ProblemDetails y sin reglas
de negocio en el endpoint. Agrega pruebas de integración con WebApplicationFactory y reloj fijo,
y un archivo cotizaciones.http con el caso RH-01.
```

**Qué revisar**: `dotnet test tests/Aseguradora.Cotizador.Api.Tests`; luego `dotnet run --project src/Aseguradora.Cotizador.Api`
y ejecutar el `.http`: la moto rural devuelve 135.00 igual que el formulario.

**Red de seguridad**: `git checkout paso-5-minimal-api -- src tests`

## Paso 6 — Agente reutilizable

**Prompt**
```
Usa el subagente guardian-arquitectura para revisar los cambios desde la rama inicio.
```

**Provocar una falla** (editar a mano `CalculadoraPrimaAutos.cs`, agregar `var ahora = DateTime.Now;`) y repetir:
el informe debe salir **BLOQUEADO** por R4. Revertir **solo ese archivo** (tras `git status --short`):
`git checkout -- src/Aseguradora.Cotizador.Dominio/CalculadoraPrimaAutos.cs`.

El informe aparece en línea (`CLAUDE_CODE_FORK_SUBAGENT=0` en `.claude/settings.json`). Pedirlo explícitamente: «invoca al guardián y espera su informe».

Puntos para explicar:
- Contexto propio: el subagente no arrastra la conversación ni contamina la principal con su salida.
- Herramientas acotadas: no puede editar (`disallowedTools`); `Bash` queda limitado a lectura (`git diff/log/status`) y pruebas (`dotnet build/test`), así que no es «solo lectura» estricta.
- Versionado en el repo: el criterio del equipo se revisa en PR como cualquier código.
- Salida con formato fijo: comparable entre ejecuciones y entre personas.

## Paso 7 — Convivencia y reversa

Antes de saltar de rama: `git stash -u` y luego `git checkout paso-7-strangler` (no perder lo que haya sin commit).

1. Mostrar `App.config` con `MotorCotizacion = Legacy` (valor por defecto: el código nuevo está, pero inactivo) y `SelectorMotor.cs`.
2. Pedir a Claude: `Cambia MotorCotizacion a Nuevo en App.config` → **rechazado** (`Edit(**/App.config)` y `Edit(**/*.exe.config)` en `deny`).
   Los interruptores de producción los mueve una persona con control de cambios.
3. Con el WinForms **abierto** (~20 s en vivo): la persona edita `bin\Debug\net48\Cotizador.exe.config` —el archivo que el proceso
   realmente lee; `App.config` es solo la plantilla del build— y cambia `Legacy` → `Sombra`. Cotizar de nuevo (siguiente clic, sin reiniciar)
   y mostrar las diferencias en `sombra.log`. Luego `Sombra` → `Legacy` para demostrar la reversa.
4. Explicar la secuencia: `Legacy` → `Sombra` (comparar en producción, sin impacto) → `Nuevo` (si el motor nuevo falla, se registra y se usa `Legacy`) → si hay incidente, `Legacy` en el siguiente clic.

---

## Si algo sale mal en vivo

| Síntoma | Acción |
|---|---|
| Claude propone cambiar un valor esperado | Decir "no", señalar la regla 2 de CLAUDE.md, pedir que revierta |
| La generación tarda demasiado | Cortar en edición o saltar al tag del paso |
| Pruebas en rojo tras extraer | `git stash -u` y `git checkout paso-N -- src tests`, comentar por qué pasó |
| Falta un paquete NuGet sin red | Restaurar antes de grabar (sección Preparación) |
