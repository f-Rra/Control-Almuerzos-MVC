# Guía Commit 13 — CRUD de Empleados Funcional

## 🎯 Propósito

Activar el módulo interactivo de carga, edición, listado cruzado ("con empresa vinculada") y baja de los Empleados, conectándolos de manera definitiva a la base de datos de producción mediante el Controlador de MVC respectivo.

## 📝 Concepto Central

El Empleado tiene una relación y validación crítica superior a la de la Empresa: **La credencial RFID**. Antes de dar de alta o editar un empleado, el controlador deberá evaluar unívocamente (y preferentemente usando un helper AJAX en caliente para el usuario antes de hacer POST completo) si esa credencial `RFxxxx` está "Libre" o ya fue instanciada por otro compañero activo en la BD.

## ⚡ Pasos a Seguir

1. **Modelo-Vista (`ViewModels/EmpleadoViewModel.cs`)**:
   - Construir una estructura similar a Empresas: Grilla completa + `Empleado` instanciado + Select combo de Empresas activas y Estados.
2. **Controlador Central (`Controllers/EmpleadoController.cs`)**:
   - Implementar CRUD base (`Index`, `Create` POST, `Edit` POST, `DeleteConfirmed`).
   - Programar la carga delegada del combo de empresas en el `Get`.
   - Construir el endpoint `VerificarCredencial(string hex)` para responder a JavaScript con un `Json` true/false.
3. **Actualización de Vistas y Scripts**:
   - Mapear las celdas `@Html.DisplayFor` con los datos traídos desde SQL.
   - En `site.js` (o en la sección Scripts de la vista), capturar el botón `Verificar` de credencial del UI, enviar Request a `/Empleado/VerificarCredencial` y actualizar el color del icono (Verde Disponible / Rojo Ilegal).

## 📁 Archivos a Crear y Modificar

| Archivo | Acción | Descripción |
|---|---|---|
| `Controllers/EmpleadoController.cs` | Crear | Endpoints y manejo de listados y filtros relacionales |
| `ViewModels/EmpleadoViewModel.cs` | Crear | Envoltura unificadora |
| `Views/Empleado/Index.cshtml` | Modificar | Data binding a controles y tabla |
| `wwwroot/js/site.js` | Modificar | Llamado asíncrono preventivo anti-colisión de credencial |

## 🚀 Mensaje de Commit

```
feat: implementar CRUD funcional de empleados

- Controlador con acciones Index, Create, Edit, DeleteConfirmed, VerificarCredencial
- Vista conectada a datos reales con Razor
- Búsqueda en tiempo real y filtro por empresa
- Validación de credencial RFID única
- ViewModels y vista parcial de formulario
```
