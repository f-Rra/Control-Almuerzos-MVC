# Guía Commit 4 — Diseño de Vista CRUD Empresas

## 🎯 Propósito

Crear la vista de gestión de Empresas (`Views/Empresa/Index.cshtml`). A diferencia de un ABM clásico estructurado en múltiples páginas, el sistema requerirá un diseño de dos columnas ("Master-Detail" inline) donde la lista de empresas está siempre visible a la izquierda, y su formulario de alta/edición aparece a la derecha.

## 📝 Concepto Central

El objetivo principal es reducir clics. El usuario administrador seleccionará una empresa de la grilla izquierda, y la misma grilla no se recargará, sino que el formulario de la derecha se poblará automáticamente en el futuro (mediante JavaScript/AJAX). Ahora realizaremos el chasis visual de estas dos columnas.

## ⚡ Pasos a Seguir

1. **Crear Vista**: Agregar `Views/Empresa/Index.cshtml`.
2. **Layout Columna Izquierda (Master)**:
   - Input de búsqueda visual con ícono.
   - Tabla estilizada (Nombre Empresa, Estado).
   - Filas "hardcodeadas" para la prueba visual.
3. **Layout Columna Derecha (Formulario)**:
   - Título dinámico "Nueva Empresa" o "Editando Empresa X".
   - Input de texto "Razón Social".
   - Radio buttons o toggle para "Activo/Inactivo".
   - Mini panel inferior estadístico (empleados totales, asistencias) para la empresa clickeada.
   - Grupo de botones de form (Guardar, Cancelar, Desactivar).

## 📁 Archivos a Crear y Modificar

| Archivo | Acción | Descripción |
|---|---|---|
| `Views/Empresa/Index.cshtml` | Crear | Interfaz Master-Detail estática para empresas |
| `wwwroot/css/site.css` | Modificar | Soporte para vista particionada (2 columnas grid) |

## 🚀 Mensaje de Commit

```
feat: diseño de vista CRUD de empresas

- Tabla de empresas con búsqueda y conteo
- Formulario de alta/edición con validación visual
- Panel de estadísticas por empresa
- Botones de acción (guardar, cancelar, eliminar)
```
