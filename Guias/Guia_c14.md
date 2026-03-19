# Guía Commit 14 — Servicio Activo y Registro por Credencial

## 🎯 Propósito

Implementar el núcleo productivo, o "músculo diario" del sistema. Esta funcionalidad permitirá crear una jornada laboral de almuerzo real vinculada a la DB, prender el cronómetro web, recepcionar credenciales RF por un lector simulando el hardware (inputs POST en caliente) y guardar sus registros individualmente en las tablas.

## 📝 Concepto Central

En un sistema web transaccional, mantener un estado de "Servicio Abierto" continuo en una sola pantalla exige coordinación Cliente-Servidor. El `ServicioController` actuará como un vigilante. Determinará si existe un servicio en memoria vivo enviando un solo objeto "ServicioActivoViewModel" hacia la vista. La vista usará el JavaScript `setInterval` para mutar el tiempo cliente simulando el pulso, e instrumentará POSTs constantes de registro individual para no cortar el estado del UI.

## ⚡ Pasos a Seguir

1. **ViewModel (`ViewModels/ServicioActivoViewModel.cs`)**:
   - Incluir prop. Servicio Instanciado y Lista comensales recién registrados.
2. **Controlador (`Controllers/ServicioController.cs`)**:
   - `Index()`: Si `IServicioNegocio.ObtenerActivo()` no devuelve null, devuelve la pantalla activa, si no, devuelve la pantalla setup.
   - `Iniciar()`: Recibe lugar/proyección general. Inserta a DB con hora inicial. Devuelve `RedirectToAction(Index)`.
   - `Finalizar()`: Acumula el tiempo Javascript transferido al servidor y da de baja o cierra en DB el flag de activo.
   - `Registrar(credencial)` (AJAX POST): Llama la SP "RegistrarComensal". Si la DB contesta "Ya está adentro", se ataja un custom toast error. Caso de éxito inserta un registro individual. Devuelve `Ok(lista)`.
3. **Integraremos Scripts (`site.js`)**:
   - Instancia el intervalo nativo para el formato de cronómetro `HH:mm:ss`.
   - Modifica los anchos de las barras `CSS %` del progreso.
   - Envía el POST de credencial asíncrono.

## 📁 Archivos a Crear y Modificar

| Archivo | Acción | Descripción |
|---|---|---|
| `ViewModels/ServicioActivoViewModel.cs` | Crear | Vista combinada |
| `Controllers/ServicioController.cs` | Crear | El centinela de concurrencias |
| `Views/Servicio/Index.cshtml` | Modificar | Estructura Razer para `if (@Model.ExisteServicioActivo)` |
| `wwwroot/js/site.js` | Modificar | Lógica completa de UI vivo y refresco asíncrono |

## 🚀 Mensaje de Commit

```
feat: implementar servicio activo y registro por credencial

- Iniciar/finalizar servicio con validaciones
- Registro de comensales por credencial con AJAX
- Cronómetro JavaScript en tiempo real
- Barra de progreso y notificaciones toast
- Listado dinámico de comensales registrados
```
