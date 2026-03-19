# Guía Commit 15 — Registro Manual Masivo

## 🎯 Propósito

Implementar y conectar al backend la alternativa obligatoria para cuando los comensales olvidan su tarjeta física: una herramienta unificada para seleccionar rápidamente por empresa o nombre y efectuar grabados masivos en SQL para el Servicio actual activo.

## 📝 Concepto Central

No basta con ser dinámico, debe ser seguro. Si dos administradores ejecutan esta pantalla desde dos navegadores a la vez tildando a los mismos usuarios, o la persona ya almorzó pasando la tarjeta, el Controlador (`RegistroController`) y su capa Negocio deben hacer uso seguro contra colisiones antes de aprobar una carga "masiva". El enfoque será 100% transaccional mediante JS y SPs.

## ⚡ Pasos a Seguir

1. **Controlador (`Controllers/RegistroController.cs`)**:
   - Acción `Index()`: Verifica y exige la existencia de un "Servicio Activo". Retorna un modelo base vacío referenciando las empresas disponibles.
   - Filtro JSON `ObtenerSinAlmorzar(idEmp)`: Devolver la tabla en bruto desde `IEmpleadoNegocio` para ser renderizada a la izquierda.
   - Endpoint Clave `RegistrarMultiples([FromBody] List<int> ids)`: Recibe múltiples Checkboxes simultáneos, lo corre en un iterador e invoca el SP individual en crudo con try/catch omitible sobre falsos positivos. Devuelve cuenta exitosa. 
2. **Scripts (`site.js`)**:
   - Funciones `CargarEmpleadosTabla(empresa, filtroTexto)`.
   - Disparador de recopilación: Función que escanea el DOM por `<input type="checkbox" checked>` e inyecta al Request `RegistrarMultiples`.
   - Contabilización paralela "Visual": Suma los elementos exitosos al display hardcodeado de Contadores.

## 📁 Archivos a Crear y Modificar

| Archivo | Acción | Descripción |
|---|---|---|
| `Controllers/RegistroController.cs` | Crear | Recepción de listas e inyección del fallback |
| `Views/Registro/Index.cshtml` | Modificar | Agregar helpers Razer al dropdown filtro |
| `wwwroot/js/site.js` | Modificar | Scripts JSON Array sender por AJAX |

## 🚀 Mensaje de Commit

```
feat: implementar registro manual de empleados

- Registro masivo vía AJAX
- Sincronización en tiempo real de cards
- Filtrado dinámico por empresa y nombre
- Notificacione integradas con showMessage
- Conteo real basado en la tabla Registros
```
