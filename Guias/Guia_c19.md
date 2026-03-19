# Guía Commit 19 — Validaciones Completas y Mensajes de Usuario

## 🎯 Propósito

Garantizar la estabilidad y seguridad operativa del formulario. Evitaremos que el usuario envíe basura al servidor añadiendo validaciones preventivas en el Frontend (Client-Side con jQuery Validation/Unobtrusive) y brindaremos una interacción más pulida y cálida a través de notificaciones globales (`Toasts`) que aparecerán gentilmente tras interacciones como un registro exitoso o un borrado.

## 📝 Concepto Central

A pesar de que el backend valide el POST (Server-Side), hacer transitar a un usuario por una carga de página solo para recibir un cartel de error "Completar Nombre" interrumpe la experiencia veloz esperada de un sistema web. Usaremos jQuery Unobtrusive para que los scripts MVC intercepten los campos nulos automáticamente. Adicionalmente, crearemos una mensajería estándar global basada en `TempData` del tipo Flash.

## ⚡ Pasos a Seguir

1. **Diccionario Consistente (`Helpers/MensajesConstantes.cs`)**:
   - Armar clase estática para no repetir stings: `MsjExitoGuardado = "Registro guardado correctamente"`.
2. **Alertas Globales de MVC (`Helpers/MensajesUI.cs`)**:
   - Crear un Helper que le inyecte extensiones de extensión (Extention Method) sólidas a `ITempDataDictionary` (ej. `TempData.MensajeExito("texto")`). Escribe un objeto serializado en un JSON simple identificando color (success, error, warn).
3. **El Layout Global (`Views/Shared/_Layout.cshtml` + Toasts)**:
   - Crear la vista parcial `_Notificaciones.cshtml`. Que lea `TempData` y si recibe JSON, inyecte un Toast animado de Bootstrap o HTML custom que desaparezca en 4 segundos (`site.js` ejecuta `setTimeout` destructivo).
   - Renderizar la parcial arriba de la etiqueta `RenderBody`.
4. **Validaciones en UI (`Views/.../Index.cshtml`)**:
   - Añadir al final del archivo las referencias nativas requeridas para los forms: `@await Html.PartialAsync("_ValidationScriptsPartial")`.
   - Incluir los `asp-validation-for` (span con error) debajo de cada Input en los formularios editables de Empresa y Empleado.

## 📁 Archivos a Crear y Modificar

| Archivo | Acción | Descripción |
|---|---|---|
| `Helpers/MensajesConstantes.cs` | Crear | Textos estandarizados en base del MVC |
| `Helpers/MensajesUI.cs` | Crear | Decoradores para escribir JSON en TempData sin boilerplate |
| `Views/Shared/_Notificaciones.cshtml` | Crear | Partial estético flotante temporal superior central |
| `Views/.../Index.cshtml` | Modificar | Incorporar el RenderSection `Scripts` unobtrusive |
| `wwwroot/js/site.js` | Modificar | Auto-Destruir notificaciones flotadas |

## 🚀 Mensaje de Commit

```
feat: agregar validaciones completas y sistema de mensajes

- Validaciones client-side con jQuery Validation
- Constantes de mensajes en español
- Helper de TempData para notificaciones
- Toast notifications globales en layout
```
