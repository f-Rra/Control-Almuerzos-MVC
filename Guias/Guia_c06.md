# Guía Commit 6 — Diseño de Vista de Estadísticas

## 🎯 Propósito

Crear la vista de estadísticas y KPIs (`Views/Estadistica/Index.cshtml`). El propósito de esta pantalla es ofrecer un "Dashboard Táctico" que el usuario administrador pueda consultar para obtener una imagen rápida de la operación del sistema mediante indicadores clave.

## 📝 Concepto Central

Los indicadores (KPIs) deben agruparse en categorías obvias: Empleados, Empresas, Servicios y Asistencias. Además, debe haber un gráfico visual o barra de progreso para identificar a las "Top 5 Empresas" con mayores consumos en el comedor, facilitando la facturación rápida o visualización de impacto.

## ⚡ Pasos a Seguir

1. **Crear Vista**: Agregar `Views/Estadistica/Index.cshtml`.
2. **Definir Grid de KPIs**: Crear un grid CSS (`.stats-grid`) ordenado en tarjetas o "Cards". 
3. **Sección Empleados**: Crear cards para "Empleados Totales", "Activos", "Inactivos". Llenar numeración con HTML estático `<span>`.
4. **Sección Asistencias**: Crear cards de "Consumo Mensual", "Promedio Diario", "Asistencias Faltantes".
5. **Panel del Top 5**: Maquetar un panel especial con barras de progreso que simulen un top 5 ordenado descendente (Ej. Gema: 84%, Roemmers: 42%).
6. **Integrar CSS**: Aplicar iconografía de Bootstrap Icons y fondos glassmorphism unificados.

## 📁 Archivos a Crear y Modificar

| Archivo | Acción | Descripción |
|---|---|---|
| `Views/Estadistica/Index.cshtml` | Crear | Pantalla de Indicadores Clave |
| `wwwroot/css/site.css` | Modificar | Reglas CSS para barras de progreso interactivas y `.stats-grid` |

## 🚀 Mensaje de Commit

```
feat: diseño de vista de estadísticas y KPIs

- Cards de KPIs por sección (empleados, empresas, servicios, asistencias)
- Tendencias y métricas mensuales
- Top 5 empresas con barras de progreso
- Indicadores de cobertura y duración promedio
```
