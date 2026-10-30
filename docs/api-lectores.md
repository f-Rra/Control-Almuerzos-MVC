# API para lectores RFID

Además de las pantallas, el sistema expone una **API REST** para lectores RFID de red (por ejemplo, un ESP32 con un módulo MFRC522 que manda por Wi-Fi el UID de la tarjeta). Un lector no tiene sesión de navegador, así que se identifica con una **clave propia** en la cabecera `X-Api-Key`.

> **Estado:** la API está verificada con tests de integración que simulan al lector, y se puede probar a mano desde Swagger. **No se probó con un lector físico.** Los lectores USB comunes, que se comportan como un teclado, ya funcionan sin la API: escriben el UID en el campo de credencial de la pantalla de Servicios.

La documentación interactiva está en **`/swagger`**: lista los endpoints, explica cada respuesta y permite probarlos desde el navegador.

## Endpoint

`POST /api/registros` registra el almuerzo del dueño de una credencial en el servicio activo del lugar donde está instalado el lector.

```bash
curl -X POST http://localhost:5080/api/registros \
  -H "X-Api-Key: sca_la-clave-del-lector" \
  -H "Content-Type: application/json" \
  -d '{"credencial": "RF001"}'
```

```json
{ "idServicio": 129, "empleado": "Juan Perez", "empresa": "Roemmers", "lugar": "Comedor", "fecha": "2026-10-26", "hora": "12:05" }
```

| Código | Cuándo |
|---|---|
| `201` | Registró al comensal |
| `400` | Falta la credencial, o viene vacía |
| `401` | Falta la clave del lector, es inválida o está revocada |
| `404` | Ninguna persona tiene esa credencial |
| `409` | Esa persona ya estaba registrada en este servicio |
| `422` | El empleado está inactivo, o no hay un servicio activo hoy en el lugar del lector |

Los errores llevan el formato estándar `application/problem+json` (estado, título y detalle).

## Cómo obtener la clave de un lector

1. Entrar como administrador y abrir **Lectores** en el menú lateral.
2. Cargar el nombre del lector y el lugar donde está instalado, y tocar **Crear y generar clave**.
3. Copiar la clave: **se muestra una sola vez**. En la base queda solo su hash SHA-256, así que no se puede recuperar; si se pierde, se revoca el lector y se crea otro.
4. Para dejar de aceptar un lector, tocar **Revocar**: su clave deja de funcionar al instante.

## Probarla desde Swagger, sin un lector físico

1. Iniciar un servicio en el lugar del lector (pantalla **Servicios**).
2. Abrir `/swagger`, tocar **Authorize** y pegar la clave.
3. En `POST /api/registros` → **Try it out**, escribir una credencial (por ejemplo `RF001`) y ejecutar.

## Cómo se decide

- El lector registra siempre en el servicio activo **de su lugar**: la clave pertenece a un lugar.
- Un servicio que quedó abierto de otro día no cuenta como el de hoy.
- La API **no acepta cookies de sesión**, solo la clave. Por eso no necesita token antiforgery, que existe para defender a quienes entran con cookie.

> Si se actualiza una base existente, hay que aplicar la migración que crea la tabla de lectores: `dotnet ef database update`.
