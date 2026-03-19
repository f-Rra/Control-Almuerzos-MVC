# Guía Commit 3 — Diseño de Vista de Reportes

## 🎯 Propósito

Crear la vista de Reportes (`Views/Reporte/Index.cshtml`), que servirá para la generación y filtrado de los distintos análisis del mes. El foco del diseño estático es construir el motor de filtros superior y la tabla de resultados adaptable.

## 📝 Concepto Central

El sistema tendrá 4 tipos de reporte diferenciados. Dependiendo del dropdown "Tipo de Reporte" que seleccione el usuario, las columnas de la tabla de abajo cambiarán. Para esta fase de diseño, se dejará maquetada visualmente la tabla que corresponde al "Reporte de Servicios" general.

## ⚡ Pasos a Seguir

1. **Crear la vista**: En `Views/Reporte/Index.cshtml`.
2. **Construir el Panel de Filtros**:
   - Selector "Desde Fecha" y "Hasta Fecha".
   - Selector unificado de "Lugar".
   - Dropdown del "Tipo de Reporte".
   - Botón visual de "Buscar/Filtrar".
3. **Maquetar la Tabla Principal**: Añadir 5 filas de datos de ejemplo (Servicio, Fecha, Duración, Proyección vs Comensales, Cobertura) para fijar el estilo de celdas.
4. **Botón de Reporte**: Incluir el botón secundario superior de "Exportar PDF" o "Enviar por Email".
5. **CSS en site.css**: Reglas unificadas para inputs condicionales.

## 📁 Archivos a Crear y Modificar

| Archivo | Acción | Descripción |
|---|---|---|
| `Views/Reporte/Index.cshtml` | Crear | Pantalla de filtros y tabla de visualización (estática) |
| `wwwroot/css/site.css` | Modificar | Estilos para filtros y alineación de botones |

## 🚀 Mensaje de Commit

```
feat: diseño de vista de reportes

- Panel de filtros (fechas, lugar, tipo de reporte)
- Tabla de resultados con datos de ejemplo
- Botón de exportación a PDF
- Soporte visual para 4 tipos de reporte
```
