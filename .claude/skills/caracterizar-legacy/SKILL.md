---
name: caracterizar-legacy
description: Genera pruebas de caracterización xUnit que fijan el comportamiento ACTUAL de código heredado (C# o VB.NET) antes de refactorizarlo. Úsala cuando se pida caracterizar, fijar comportamiento, crear un golden master o "poner red" antes de mover lógica legacy.
argument-hint: "<clase o método heredado a caracterizar>"
allowed-tools: Read, Grep, Glob, Edit(/tests/**), Bash(dotnet test:*), Bash(dotnet build:*)
---

# Caracterizar código heredado

Objetivo: dejar una red de pruebas que falle ante **cualquier** cambio de comportamiento observable
del código indicado en `$ARGUMENTS`. No mejorar, no corregir, no opinar sobre si el resultado es correcto.

## Procedimiento

1. **Inventario de entradas y salidas.** Lee el código y lista:
   - entradas explícitas (parámetros) e implícitas (`DateTime.Now`, configuración, base de datos, controles de UI, estado estático);
   - salidas observables (valor de retorno, propiedades, efectos laterales).
   Si hay entradas implícitas sin costura, **detente** y propone la costura mínima (extraer método / parámetro) en lugar de escribir pruebas frágiles.

2. **Mapa de ramas.** Enumera cada `if`/`Select Case`/`switch` y sus límites. Por cada condición genera:
   - un caso dentro de la rama, uno fuera y **uno exactamente en el límite** (`>` vs `>=` es donde se esconden las rarezas);
   - casos para valores "de respaldo" (`Case Else`, `default`).

3. **Busca rarezas.** Señala cualquier comportamiento que contradiga comentarios, manuales o la intuición:
   orden de operaciones, redondeo (`Math.Round` es bancario por defecto), cálculos de edad/fechas, mínimos y topes.
   Cada rareza lleva `[Trait("Rareza", "RH-0N")]` y un nombre de prueba que describa el comportamiento real.
   Si la rareza no está en el README, márcala como **nueva** en tu resumen y pide confirmación.

4. **Valores esperados = lo que el código devuelve hoy.** Calcúlalos ejecutando el código heredado
   (o recorriéndolo paso a paso y mostrando la aritmética). Nunca los derives de lo que "debería" pasar.

5. **Estructura de las pruebas.**
   - Una clase base abstracta con las pruebas y un método abstracto `Cotizar(...)` (o equivalente).
   - Una subclase por implementación (`Legacy...Tests`). Así las mismas pruebas se reutilizan contra el código nuevo.
   - Fecha fija (`Hoy`) inyectada; nada de `DateTime.Now` en las pruebas.
   - Nombres en español con guiones bajos: `Rareza_prima_minima_se_aplica_antes_del_descuento`.
   - Datos 100 % sintéticos.

6. **Ejecuta** `dotnet test` y confirma verde contra el código heredado **sin haberlo modificado**.

7. **Mutación manual rápida.** Cambia mentalmente un límite (`>` → `>=`) o el modo de redondeo e indica qué prueba
   lo detectaría. Si ninguna lo detecta, agrega el caso que falta.

## Entrega
- Archivos de prueba creados o modificados.
- Tabla: rama / límite → prueba que la cubre.
- Lista de rarezas (conocidas y nuevas).
- Salida resumida de `dotnet test`.
