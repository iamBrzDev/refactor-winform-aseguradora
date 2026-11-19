# Riesgos, gobierno de la información y reversa

## 1. Qué se puede compartir con el asistente

| Nivel | Ejemplos | Regla |
|---|---|---|
| Verde | Código de negocio sin datos, pruebas con datos sintéticos, esquemas sin datos, logs anonimizados | Se trabaja con normalidad bajo un plan comercial |
| Amarillo | Algoritmos actuariales diferenciadores, nombres de clientes corporativos en el código, consultas con lógica de pricing | Solo con aprobación del líder técnico y en un plan comercial con la retención acordada |
| Rojo | Credenciales, cadenas de conexión, certificados, datos personales de asegurados, siniestros reales, extractos de producción | Nunca. Bloqueado por `deny`, `.gitignore` y fuera del árbol de trabajo |

Datos verificables (documentación oficial de Claude Code, *Data usage*):

- En planes **comerciales** (Team, Enterprise, API y plataformas de terceros) Anthropic no entrena modelos con el código ni los prompts enviados desde Claude Code, salvo que el cliente se inscriba expresamente en un programa para ello.
- En planes **de consumo** (Free, Pro, Max) el uso para entrenamiento depende de una opción del usuario. **Código propietario de la empresa: solo en plan comercial.**
- Retención estándar comercial: 30 días. *Zero Data Retention* disponible para cuentas elegibles de Enterprise.
- Claude Code guarda localmente las transcripciones en texto plano en `~/.claude/projects/` (30 días por defecto, ajustable con `cleanupPeriodDays`): esa carpeta también es información sensible.
- Telemetría desactivable con `DISABLE_TELEMETRY=1` o todo el tráfico no esencial con `CLAUDE_CODE_DISABLE_NONESSENTIAL_TRAFFIC=1`.
- Opciones de residencia: Amazon Bedrock, Google Cloud o Microsoft Foundry si la política exige que el tráfico no salga del proveedor de nube corporativo.

Fuente: https://code.claude.com/docs/en/data-usage

## 2. Capas de control en este repositorio

1. `.gitignore`: los secretos no entran al repositorio.
2. `secrets.config` vía `configSource`: la cadena de conexión no está en `App.config`.
3. `.claude/settings.json`: `deny` de lectura para secretos y datos de producción, `deny` de edición sobre `App.config` y `*.exe.config`, `ask` para tocar `src/Legacy/**`, `push` y `checkout`.
4. `CLAUDE.md`: reglas de conducta (no tocar valores esperados, datos sintéticos, detenerse ante credenciales). Orienta, **no obliga**: por eso existen las capas 3 y 5.
5. Límite conocido: las reglas `deny` cubren las herramientas de archivos y comandos como `cat` o `head`, pero no un proceso que abra archivos por su cuenta. Para aislamiento a nivel de sistema operativo: sandbox de Claude Code o contenedor de desarrollo.
6. Ojo con `allow`: `Bash(dotnet build *)` y `Bash(dotnet test *)` ejecutan código del repositorio (targets de MSBuild, analizadores, el código de las pruebas) **fuera** de lo que las reglas `deny` pueden ver. Por eso se combinan con sandbox y con la revisión de persona del diff (sección 3), y no se aprueban en repositorios no confiables.

## 3. Cómo se revisa el código generado

- **Pruebas primero**: el código generado se acepta porque pasa pruebas que existían **antes** de generarlo.
- **Diff pequeño**: un propósito por commit; si el diff no se entiende en 5 minutos, se parte.
- **Revisión de persona obligatoria** en el PR, con foco en: orden de operaciones, redondeo, límites (`>` vs `>=`), manejo de nulos y fechas.
- **Guardián automático**: el subagente `guardian-arquitectura` y las pruebas de arquitectura en CI.
- **Señales de alerta**: pruebas modificadas en el mismo commit que la lógica, valores esperados "corregidos", `catch` vacíos nuevos, dependencias agregadas sin pedirlas.

## 4. Cuándo NO conviene usar el asistente

- Cuando no hay forma de verificar el resultado (sin pruebas ni oráculo): primero se construye la red.
- Decisiones de negocio disfrazadas de refactor (corregir RH-01 cambia lo que se cobra).
- Código con datos de nivel rojo que no se pueden sintetizar.
- Incidentes en producción bajo presión, si el equipo no domina la herramienta: usarla para leer y diagnosticar, no para aplicar cambios a ciegas.
- Cuando el costo de explicar el contexto supera el de hacer el cambio (renombres triviales, ajustes de una línea).

## 5. Reversa sin interrumpir la operación

| Nivel | Mecanismo | Tiempo | Interrupción |
|---|---|---|---|
| Comportamiento | `MotorCotizacion` → `Legacy` en `Cotizador.exe.config` (con la app abierta en desarrollo: `bin\Debug\net48\Cotizador.exe.config`, **no** `App.config`) | Siguiente clic | Ninguna |
| Código | `git revert <commit>` del paso problemático + despliegue normal | Lo que dure el pipeline | Ninguna (el interruptor ya cubrió el incidente) |
| API | El formulario no depende de la API: se retira o se versiona (`/api/v2`) sin tocar ventanilla | Inmediato | Ninguna para ventanilla |

Secuencia de salida a producción recomendada:
1. Desplegar con `Legacy` (el código nuevo está presente pero inactivo).
2. Pasar a `Sombra` en un grupo de sucursales; revisar las diferencias registradas (`[SOMBRA]`) en `sombra.log` durante un ciclo de facturación.
3. Diferencias = 0 → `Nuevo` por grupos (si el motor nuevo falla, se registra y se responde con `Legacy`).
4. Retirar el motor heredado solo cuando `Nuevo` cumpla un ciclo completo sin incidentes.

Requisitos para compilar y probar: Visual Studio 2026 (18.0+) o `dotnet` CLI para `net10.0`.
