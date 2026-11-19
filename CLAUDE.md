# CLAUDE.md — Cotizador de Autos (núcleo asegurador)

## Contexto
- Sistema heredado en producción: WinForms sobre .NET Framework 4.8 (C# 7.3) + módulos VB.NET.
- Estamos migrando la lógica de negocio a arquitectura limpia **sin detener la operación** (patrón strangler).
- Equipo senior. Responde en español, directo, sin explicar fundamentos.

## Comandos
- Pruebas de caracterización: `dotnet test tests/Aseguradora.Cotizador.Caracterizacion.Tests`
- Pruebas de arquitectura: `dotnet test tests/Aseguradora.Cotizador.Arquitectura.Tests`
- Pruebas de la API: `dotnet test tests/Aseguradora.Cotizador.Api.Tests`
- Levantar la API: `dotnet run --project src/Aseguradora.Cotizador.Api`
- El WinForms (`src/Legacy/**`) solo compila en Windows: no intentes ejecutarlo.

## Arquitectura (regla de dependencia: siempre hacia adentro)
- `Dominio` → no depende de nada (ni de otros proyectos, ni de System.Data, ni de ASP.NET, ni del código Legacy).
- `Aplicacion` → solo `Dominio`. Define puertos (`IReloj`, `IRepositorioCotizaciones`).
- `Infraestructura` → `Aplicacion` y `Dominio`. Implementa los puertos.
- `Api` y `WinForms` → son bordes; pueden depender de todo lo anterior, nadie depende de ellos.
- `Dominio`, `Aplicacion` e `Infraestructura` son `netstandard2.0`: sin `record`, sin `init`, sin APIs de .NET moderno.
- Prohibido `DateTime.Now`/`DateTime.Today` en Dominio y Aplicacion: la fecha entra por `IReloj` o por parámetro.

## Reglas de refactor (no negociables)
1. **Primero caracterizar, después mover.** No se toca lógica sin pruebas que fijen el comportamiento actual.
2. Una prueba de caracterización registra lo que el sistema **hace**, no lo que "debería" hacer.
   Nunca cambies un valor esperado para que una prueba pase: detente y avísame.
3. Las rarezas aceptadas (RH-01 a RH-04, ver README) se **preservan** y se nombran explícitamente en el dominio.
   Si detectas una rareza nueva, documéntala como RH-0N y pregúntame antes de seguir.
4. En `src/Legacy/**` solo se permiten refactorizaciones mecánicas (mover/extraer) que no cambien comportamiento,
   y cada una debe ir sola en su commit. Excepción explícita: los **puntos de extensión del patrón Strangler**
   (selector de motor, adaptador hacia el dominio, interruptor de configuración) aprobados por ticket y
   referenciados en el archivo o en el commit; nunca tocan las expresiones del cálculo heredado.
5. Pasos pequeños: después de cada cambio corre las pruebas. Si algo queda en rojo, revierte tu último cambio antes de intentar otra cosa.

## Flujo de trabajo
- Para cambios de más de un archivo: primero propón un plan (modo plan) y espera mi aprobación.
- Commits convencionales en español: `test:`, `refactor:`, `feat:`, `chore:`, `docs:`. Un propósito por commit.
- No hagas `git push` ni agregues paquetes NuGet sin preguntar.
- Al terminar una extracción, invoca explícitamente al subagente `guardian-arquitectura`, espera su informe en línea
  y muéstralo completo antes de continuar.

## Información sensible
- Nunca leas ni muestres `secrets.config`, `*.pfx`, `.env*`, `appsettings.*.local.json` ni `datos-produccion/`.
- No pegues datos reales de asegurados en prompts, pruebas ni ejemplos: usa datos sintéticos.
- Si un archivo parece contener credenciales o datos personales, detente y avísame.

## Definición de terminado
- Pruebas de caracterización, arquitectura y API en verde.
- Sin cambios de comportamiento observables (salvo decisión de negocio documentada).
- Informe del `guardian-arquitectura` sin hallazgos bloqueantes.
- Diff revisado por una persona antes del merge.
