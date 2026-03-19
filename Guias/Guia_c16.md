# Guía Commit 16 — Reportes y Exportación a PDF

## 🎯 Propósito

Implementar el motor de análisis y reportes del sistema. Esta vista debe permitir aplicar 4 filtros distintos (fechas, lugar, tipo de reporte) para que los datos operativos brutos se transformen en información gerencial, con la posibilidad de descargarla directamente en PDF mediante la librería QuestPDF.

## 📝 Concepto Central

Puesto que existen 4 tipos estructurales de reporte (cada uno con columnas completamente distintas, como "Total vs Proyección" o "Días de la semana"), el Controlador unificará la solicitud (`ViewModels/ReporteViewModel.cs`), determinará lógicamente qué SP de SQL llamar (`IReporteNegocio`), pasará los datos formateados y seleccionará una Vista Parcial Dinámica o construirá el Documento PDF exacto requerido por el usuario sin recargas forzadas innecesarias en caso de fallo.

## ⚡ Pasos a Seguir

1. **Modelos y Request (`Models/Reportes.cs` + `ViewModels/ReporteViewModel.cs`)**:
   - Codificar las 4 clasificaciones y crear sus atributos de propiedad C#.
   - Unificar los filtros (Desde, Hasta, Lugar, Tipo) en el Modelo-Vista.
2. **Capa Negocio y Controller (`Controllers/ReporteController.cs` + `Services/ReporteNegocio.cs`)**:
   - Implementar `ObtenerListaServicios`, `ObtenerAsistenciasEmpresa`, `ObtenerCobertura`, `ObtenerDistribucion`.
   - Modificar la Acción `Generar()` (POST) para que valide fechas absurdas y despache la obtención del modelo a Negocio. Retorna al bloque de resultados del frontend (o recarga total cargando el `Model.Resultados`).
3. **Módulo PDF (`Services/PdfService.cs` y `SCA-MVC.csproj`)**:
   - Ejecutar `dotnet add package QuestPDF` (Licencia Community libre o Evaluation). 
   - Crear una clase que herede de `IDocument` (Documento QuestPDF paramétrico) o un helper genérico para dibujar el título, los filtros insertados (ej: "Desde enero hasta marzo"), una tabla blanca con cabecera gris, y los valores iterados provenientes de la lista casteada.
   - Endpoint `ExportarPdf(filtros)` que retorne el `File(bytes, "application/pdf", "Reporte.pdf")`.

## 📁 Archivos a Crear y Modificar

| Archivo | Acción | Descripción |
|---|---|---|
| `SCA-MVC.csproj` | Modificar | Incluir paquete NuGet `QuestPDF` |
| `ViewModels/ReporteViewModel.cs` | Crear | Transporte de filtros y resultados variables |
| `Services/ReporteNegocio.cs` | Crear | Ejecutores de SP múltiples por TipoReporte Enum |
| `Services/PdfService.cs` | Crear | Renderizador backend para gráficos/tablas en buffer binario |
| `Controllers/ReporteController.cs` | Crear | Intermediador visual |
| `Views/Reporte/Index.cshtml` | Modificar | Estructura Razer dinámica `@if (Model.Tipo == 1)` |

## 🚀 Mensaje de Commit

```
feat: implementar reportes con filtros y exportación PDF

- 4 tipos de reporte (servicios, empresas, cobertura, distribución)
- Filtros por fecha, lugar y tipo de reporte
- Generación de PDF con QuestPDF (header, tabla formateada)
- Modelos de datos para reportes
```
