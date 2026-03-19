# Guía Commit 2 — Diseño de Vista de Registro Manual

## 🎯 Propósito

Crear la vista de registro manual (`Views/Registro/Index.cshtml`) que funcionará como método alternativo (backup) al registro por credencial. Se realizará el maquetado visual con datos estáticos, enfocándose en la interacción de selección múltiple (checkboxes) y filtros de búsqueda.

## 📝 Concepto Central

A veces los operarios olvidan su credencial. El registro manual permite buscar visualmente a un empleado o filtrar por empresa, tildar varios checkboxes en una tabla y registrarlos masivamente de un solo clic. El diseño debe enfatizar los filtros en la parte superior, la tabla en el centro izquierdo, y un panel de estado ("Seleccionados, Registrados, Pendientes") siempre visible en el lateral derecho para guiar al usuario.

## ⚡ Pasos a Seguir

1. **Crear la vista**: En `Views/Registro/Index.cshtml`.
2. **Definir Layout Modular**: Construir una estructura a dos columnas (Tabla + Sidebar de Resumen).
3. **Barra de Herramientas**: Implementar un `<select>` para empresas y un `input` de texto para la búsqueda por el nombre.
4. **Tabla de Selección**: Maquetar la tabla de empleados, donde la primera columna es un checkbox. Incluir nombres y empresas ficticios para previsualización.
5. **Panel Lateral**: Crear las "cards" de resumen con números hardcodeados y el botón principal de "Agregar Seleccionados".
6. **Estilos HTML**: Adaptar clases CSS para que coincida con el tema glassmorphism de la aplicación.

## 📁 Archivos a Crear y Modificar

| Archivo | Acción | Descripción |
|---|---|---|
| `Views/Registro/Index.cshtml` | Crear | Vista de registro masivo alternativo (diseño estático) |
| `wwwroot/css/site.css` | Modificar | Ajustes de layout para el sidebar y tabla de checkboxes |

## 🚀 Mensaje de Commit

```
feat: diseño de vista de registro manual

- Filtros por empresa y búsqueda por nombre
- Tabla de empleados sin almorzar con selección múltiple
- Botón de registro masivo
- Resumen de registrados vs pendientes
```
