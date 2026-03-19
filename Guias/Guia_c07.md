# Guía Commit 7 — Diseño de Vista del Home Administrador

## 🎯 Propósito

Crear la vista de inicio exclusiva para el rol administrador (`Views/Admin/Index.cshtml`). Esta pantalla será su primer punto de entrada, funcionando como un puente o hub en lugar de forzarlo a navegar el menú lateral derecho de inmediato.

## 📝 Concepto Central

En lugar de mostrar las funciones operativas del día a día, la vista del Admin prioriza enlaces rápidos y concisos a las secciones administrativas (Configuración de Empresas, de Empleados, Usuarios, y Estadísticas). El diseño consistirá en grandes tarjetas clickeables con íconos distintivos.

## ⚡ Pasos a Seguir

1. **Crear Controlador**: Agregar `Controllers/AdminController.cs` con su acción base `Index()`.
2. **Crear Vista**: En `Views/Admin/Index.cshtml`.
3. **Encabezado y Bienvenida**: Añadir un mensaje simple que diga "Panel de Administración".
4. **Grid Central (`.adm-grid`)**: Maquetar un contenedor flexible de 3 o 4 tarjetas centrales enormes.
5. **Tarjetas (Cards)**:
   - "Empresas" (ícono edificio, enlace a `/Empresa`).
   - "Empleados" (ícono personas, enlace a `/Empleado`).
   - "Estadísticas" (ícono gráfico, enlace a `/Estadistica`).
6. **Diseño CSS (`adm-card`)**: Incorporar un hover dinámico para invitar al click.

## 📁 Archivos a Crear y Modificar

| Archivo | Acción | Descripción |
|---|---|---|
| `Views/Admin/Index.cshtml` | Crear | Vista Hub o entrada del administrador |
| `Controllers/AdminController.cs` | Crear | Controlador router para la sección Admin |
| `wwwroot/css/site.css` | Modificar | Estilos dedicados (`.adm-grid`, `.adm-card`) |

## 🚀 Mensaje de Commit

```
feat: diseño de vista de inicio del administrador

- Hub de acceso rápido a Empresas, Empleados y Estadísticas
- Grid de cards con íconos, conteos e indicadores
- Controlador AdminController con acción Index
- Estilos adm-* integrados en site.css
```
