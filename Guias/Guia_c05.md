# Guía Commit 5 — Diseño de Vista CRUD Empleados

## 🎯 Propósito

Crear la vista de gestión de Empleados (`Views/Empleado/Index.cshtml`). Esta vista será la pantalla más interactiva del ABM, utilizando el mismo concepto de dos columnas diseñado para Empresas, pero adaptado a una mayor cantidad de datos y filtros. Se construirá toda su maquetación base.

## 📝 Concepto Central

Al igual que en Empresas, buscamos la técnica Master-Detail, pero el listado de la izquierda requiere filtros avanzados (búsqueda por nombre y select de la empresa a la que pertenecen). El formulario derecho necesita incorporar un campo crítico: la credencial de validación RFID, que eventualmente requerirá un botón visual embebido en el mismo input para chequear disponibilidad.

## ⚡ Pasos a Seguir

1. **Crear Vista**: Ubicada en `Views/Empleado/Index.cshtml`.
2. **Columna de Listado (Master)**:
   - Filtro dual (Input Nombre + Select Empresa).
   - Tabla de 4 columnas (Credencial, Nombre, Empresa, Estado).
   - Sembrado de 8 filas de ejemplo en HTML plano para la maqueta.
3. **Columna Formulario (Detail)**:
   - Input group para Credencial con botón de "Verificar".
   - Inputs "Nombre" y "Apellido".
   - Select de asignación de Empresa.
   - Estado "Activo/Inactivo" mediante radios.
   - Botonera unificada de acción.
4. **CSS**: Asegurar en `site.css` que los tamaños de input y botones sean equitativos al de las otras pestañas administrativas.

## 📁 Archivos a Crear y Modificar

| Archivo | Acción | Descripción |
|---|---|---|
| `Views/Empleado/Index.cshtml` | Crear | Vista de administración particionada adaptada a Empleados |
| `wwwroot/css/site.css` | Modificar | Estilos para inputs unificados de credencial, tablas |

## 🚀 Mensaje de Commit

```
feat: diseño de vista CRUD de empleados

- Tabla de empleados con filtros por nombre y empresa
- Formulario con verificación de credencial RFID
- Combo de empresas y selector de estado
- Botones de acción (nuevo, guardar, eliminar, cancelar)
```
