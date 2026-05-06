# Guía Commit 52 — Corrección de bugs y consistencia en controllers

## Commit
`fix: corregir autorización, TempData y async en controllers`

---

## Contexto

Revisión exhaustiva del proyecto previa al deploy detectó cuatro problemas reales en los controllers: uno de seguridad, uno de consistencia, uno de rendimiento y uno de documentación. Este commit los corrige sin tocar lógica de negocio ni estructura de vistas.

---

## Correcciones aplicadas

### 1. `ReporteController` — autorización incorrecta

**Problema:** `[Authorize]` permitía acceso a cualquier usuario autenticado. Según la arquitectura del sistema, los reportes son exclusivos del rol Admin.

**Corrección:** `[Authorize]` → `[Authorize(Roles = "Admin")]`, consistente con `EmpresaController`, `EmpleadoController` y `AdminController`.

---

### 2. `EmpresaController` — TempData con claves raw

**Problema:** Los tres métodos de acción (Create, Edit, Delete) usaban `TempData["ToastType"]`, `TempData["ToastTitle"]` y `TempData["ToastMessage"]` directamente. Todos los demás controllers del proyecto usan los métodos de extensión de `Helpers/MensajesUI.cs`.

**Corrección:** Reemplazados por `TempData.MostrarExito()`, `TempData.MostrarError()` y `TempData.MostrarAdvertencia()`. Ahora `EmpresaController` es consistente con el resto del sistema.

---

### 3. `EmpresaController.EmpresaExiste()` — bloqueo síncrono

**Problema:** El método privado usaba `.Result` sobre una llamada async, lo que bloquea el hilo de forma síncrona y puede causar deadlock en ASP.NET Core bajo carga.

```csharp
// Antes — bloqueo síncrono
private bool EmpresaExiste(int id) =>
    _empresaNegocio.BuscarPorIdAsync(id).Result != null;

// Después — async correcto
private async Task<bool> EmpresaExisteAsync(int id) =>
    await _empresaNegocio.BuscarPorIdAsync(id) != null;
```

---

### 4. `Empresa.cs` — comentario incorrecto

El comentario `// Propiedad calculada (no mapeada a BD)` sobre `CantidadEmpleados` era falso: la propiedad SÍ está mapeada y tiene columna en la BD desde la migración inicial. Se eliminó el comentario.

---

## Herramientas de IA utilizadas

Claude Code identificó los cuatro hallazgos durante una revisión exhaustiva del proyecto con un agente especializado. Los cambios fueron aplicados de forma quirúrgica sin tocar código adyacente.
