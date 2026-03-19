# Guía Commit 12 — CRUD de Empresas Funcional

## 🎯 Propósito

Transformar el diseño estático de "Empresas" (hecho en el Commit 4) en un módulo dinámico funcional con operaciones reales a la base de datos (ADO.NET), incorporando las convenciones del cursado MVC (ViewModels y acciones estructuradas).

## 📝 Concepto Central

Construiremos un `EmpresaController` que conectará la UI con los servicios de base de datos (`IEmpresaNegocio`). A diferencia del scaffold normal, el patrón de "formulario lateral" de este sistema requiere actualizar dinámicamente el panel de la derecha tras la interacción (vía MVC estándar / recargas dirigidas) pero manteniendo el filtrado a la izquierda. Usaremos ViewModels unificados sin `[Bind]`.

## ⚡ Pasos a Seguir

1. **Configurar el Modelo-Vista (`ViewModels/EmpresaViewModel.cs`)**:
   - Agrupar la "Grilla" (`IEnumerable<Empresa>`) y el "Formulario Activo" (`Empresa`) en una misma clase envoltura para que la vista renderice todo en una pasada.
2. **El Controlador (`Controllers/EmpresaController.cs`)**:
   - Inyectar `IEmpresaNegocio`.
   - Programar `Index()` cargando la grilla.
   - Programar acciones POST: `Create` y `Edit`, con sus respectivos bloques `try/catch` y redireccionamiento `RedirectToAction(nameof(Index))`.
   - Programar `DeleteConfirmed` con su etiqueta unificadora `[HttpPost, ActionName("Delete")]` que realice una "baja lógica".
   - Crear el sub-bloque privado `EmpresaExiste()`.
3. **Reescribir Vista**:
   - Conectar Modelos (reemplazar etiquetas duras por `@foreach (var emp in Model.Lista)` y anclar form tags Helpers `asp-action`, `asp-for`).
   - Crear script JavaScript `site.js` para cargar el formulario dinámicamente (panel lateral).

## 📁 Archivos a Crear y Modificar

| Archivo | Acción | Descripción |
|---|---|---|
| `Controllers/EmpresaController.cs` | Crear | Lógica de enrutamiento y operaciones sobre SPs |
| `ViewModels/EmpresaViewModel.cs` | Crear | Envoltura de datos de grilla + formulario |
| `Views/Empresa/Index.cshtml` | Modificar | Aplicación Razor de Data Binding |
| `wwwroot/js/site.js` | Modificar | Script AJAX simple para cargar formulario clickeado |

## 🚀 Mensaje de Commit

```
feat: implementar CRUD funcional de empresa

- Controlador con acciones Index, Create, Edit, Delete
- Vista conectada a datos reales con Razor
- Búsqueda en tiempo real y panel de estadísticas
- Validaciones server-side
- ViewModels para la vista
```
