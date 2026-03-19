# Guía Commit 8 — Navegación Activa y Limpieza de Layout

## 🎯 Propósito

Refinar la usabilidad del Sidebar (Layout) integrando navegación activa persistente y limpiar el código "basura" o el scaffold inicial que viene por defecto al crear el proyecto ASP.NET MVC.

## 📝 Concepto Central

Cuando un usuario hace clic en "Reportes", el sidebar debe remarcar ese botón visualmente para indicarle "estás aquí". Esto se obtiene analizando el nombre del controlador actual desde el servidor (`ViewContext.RouteData`). Además, las vistas genéricas `Privacy` y su respectiva regla en CSS no son requeridas por nuestra app.

## ⚡ Pasos a Seguir

1. **Limpieza del Controller**: En `Controllers/HomeController.cs` eliminar la acción `Privacy()`.
2. **Remover Vista Muerta**: Eliminar físicamente el archivo `Views/Home/Privacy.cshtml`.
3. **Limpieza de CSS Aislado**: Eliminar o vaciar el archivo `_Layout.cshtml.css` que carga propiedades genéricas por defecto del template de .NET.
4. **Navegación Activa en `_Layout.cshtml`**:
   - Usar Razor: `var controlador = ViewContext.RouteData.Values["controller"]?.ToString();`
   - En cada enlace `<a>` del sidebar, comparar la variable: `class="mi @(controlador == "Servicio" ? "active" : "")"`.
   - Modificar la etiqueta genérica "Configuración" por "Estadísticas".

## 📁 Archivos a Modificar y Eliminar

| Archivo | Acción | Descripción |
|---|---|---|
| `Controllers/HomeController.cs` | Modificar | Desvincular vista de privacidad |
| `Views/Home/Privacy.cshtml` | Eliminar | Archivo obsoleto del framework |
| `Views/Shared/_Layout.cshtml.css` | Modificar | Limpieza total de atributos scaffold |
| `Views/Shared/_Layout.cshtml` | Modificar | Lógica de resaltado "active" de links en Sidebar |

## 🚀 Mensaje de Commit

```
feat: completar limpieza de navegación base y vistas scaffold

- Sidebar con navegación activa alineada al dominio (Estadísticas)
- Eliminada acción y vista Privacy del Home scaffold
- Limpieza de estilos scaffold no utilizados en _Layout.cshtml.css
```
