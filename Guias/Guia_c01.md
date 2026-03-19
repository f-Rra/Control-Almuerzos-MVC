# Guía Commit 1 — Diseño de Vista de Servicios

## 🎯 Propósito

Crear la vista principal de servicios (`Views/Servicio/Index.cshtml`) que unifica la configuración e inicio de servicio (panel superior) con el registro en tiempo real. Esta vista será estática por ahora, maquetando la estructura visual con datos "hardcodeados" para establecer el diseño glassmorphism adaptativo antes de agregar la lógica del servidor.

## 📝 Concepto Central

El diseño debe adaptarse a dos estados mutuamente excluyentes:
1. **Sin servicio activo**: Se muestra solo el panel de configuración central para que el usuario ingrese la fecha, proyección y lugar para iniciar la jornada.
2. **Con servicio activo**: El panel de configuración se bloquea en modo "solo lectura" (informando qué servicio está corriendo) y se despliegan debajo los paneles de cronómetro, progreso de la cobertura, barra de registro por credencial RFID y la tabla de comensales.

## ⚡ Pasos a Seguir

1. **Crear la vista**: Construir el archivo `Index.cshtml` en `Views/Servicio/`.
2. **Aplicar diseño base**: Maquetar un contenedor principal dividido en secciones usando componentes estáticos que emulen datos reales (ej. "76% de cobertura", "Duración: 01:24:10").
3. **Sección Configuración**: Crear el formulario con `select` para lugar, y campos para proyección e invitados.
4. **Sección Activa**: Crear el layout del cronómetro grande y barras de progreso.
5. **Sección Tabla**: Crear la estructura de la tabla de listado de comensales.
6. **Estilos**: Añadir las reglas CSS necesarias en `site.css` para respetar la estética general.

## 📁 Archivos a Crear y Modificar

| Archivo | Acción | Descripción |
|---|---|---|
| `Views/Servicio/Index.cshtml` | Crear | Vista principal maquetada con datos estáticos |
| `wwwroot/css/site.css` | Modificar | Estilos específicos para esta vista adaptativa |

## 🚀 Mensaje de Commit

```
feat: diseño de vista de servicios

- Panel de configuración (lugar, fecha, proyección, invitados, iniciar/finalizar)
- Panel de estado del servicio activo (cronómetro, cobertura, registrados/faltan)
- Campo de registro por credencial RFID
- Listado de comensales registrados en el servicio
- Vista adaptativa según estado del servicio (activo/inactivo)
```
