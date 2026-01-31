# Unidad 6: Características Avanzadas y Optimización

## Introducción

En esta unidad final aprenderás:
- AJAX para actualización en tiempo real
- SignalR para notificaciones en vivo
- Optimización de rendimiento
- Buenas prácticas de desarrollo
- Preparación para producción
- Despliegue de la aplicación

---

## AJAX y Actualización en Tiempo Real

### ¿Qué es AJAX?

**AJAX (Asynchronous JavaScript and XML)** permite actualizar partes de una página web sin recargarla completamente.

**Ventajas:**
- Mejor experiencia de usuario
- Menor consumo de ancho de banda
- Respuesta más rápida
- Interfaz más fluida

### Casos de Uso en tu Sistema

**1. Registro de comensales:**
- Actualizar contador sin recargar página
- Agregar fila a la tabla en tiempo real
- Mostrar notificación de éxito

**2. Búsqueda de empleados:**
- Filtrar mientras se escribe
- Mostrar resultados instantáneamente
- Autocompletado

**3. Validación de credencial:**
- Verificar disponibilidad en tiempo real
- Mostrar mensaje de disponible/no disponible
- Feedback inmediato

### Implementación de AJAX

#### Búsqueda de Empleados con AJAX

**Controlador:**

**Acción BuscarEmpleados:**
- Recibe término de búsqueda
- Filtra empleados por nombre, apellido o credencial
- Retorna JSON con resultados limitados (ej: 10)
- Incluye información necesaria (id, nombre, empresa)

**Vista con jQuery:**

**Campo de búsqueda:**
- Input text con evento 'input'
- Debounce de 300ms para no saturar servidor
- Llamada AJAX al controlador
- Mostrar resultados en div

**Características:**
- Búsqueda mientras se escribe
- Mínimo 2 caracteres para buscar
- Limpiar resultados si está vacío
- Mostrar "No se encontraron resultados"

#### Verificación de Credencial

**Controlador:**

**Acción VerificarCredencial:**
- Recibe credencial e idEmpleado (opcional)
- Verifica si existe en la base de datos
- Excluye empleado actual si se está editando
- Retorna JSON con disponibilidad

**Vista:**

**Campo de credencial:**
- Evento 'input' en el campo
- Llamada AJAX al verificar
- Mostrar ✓ si está disponible (verde)
- Mostrar ✗ si ya existe (rojo)
- Actualizar en tiempo real

---

## SignalR para Notificaciones en Tiempo Real

### ¿Qué es SignalR?

**SignalR** es una biblioteca que permite comunicación en tiempo real entre servidor y cliente usando WebSockets.

**Casos de uso:**
- Actualización del contador de comensales en tiempo real
- Notificaciones de nuevos registros
- Sincronización entre múltiples usuarios

### Configuración

**Instalación:**
- Paquete NuGet: Microsoft.AspNetCore.SignalR

**Hub de SignalR:**
- Crear clase que herede de Hub
- Métodos para enviar notificaciones
- Clientes conectados

**Configuración en Program.cs:**
- Agregar servicio AddSignalR()
- Mapear hub con MapHub()

### Caso de Uso: Actualización del Contador

**Flujo:**
1. Usuario registra comensal
2. Controlador guarda en BD
3. Controlador notifica a todos los clientes vía SignalR
4. Todos los navegadores conectados actualizan el contador

**Hub:**
- Método NotificarNuevoRegistro
- Envía idServicio y totalComensales
- Broadcast a todos los clientes

**Controlador:**
- Inyecta IHubContext<ServicioHub>
- Después de guardar, llama a NotificarNuevoRegistro
- Todos los clientes reciben la actualización

**Cliente JavaScript:**
- Conexión al hub
- Escuchar evento "ActualizarContador"
- Actualizar DOM cuando se recibe notificación
- Actualizar contador y barra de progreso

---

## Optimización de Rendimiento

### 1. Consultas Eficientes

**Problema N+1:**
- Ocurre cuando se hace una consulta por cada elemento
- Muy ineficiente con muchos registros

**Solución:**
- Usar Include() para eager loading
- Cargar relaciones en una sola consulta
- Evitar lazy loading en aplicaciones web

**Ejemplo:**
- En vez de cargar empleados y luego empresa de cada uno
- Cargar empleados con Include(e => e.Empresa)

### 2. Paginación

**Problema:**
- Cargar miles de registros es lento
- Consume mucha memoria
- Mala experiencia de usuario

**Solución:**
- Implementar paginación
- Mostrar 20-50 registros por página
- Usar Skip() y Take()

**Implementación:**
- Parámetro de página en la acción
- Calcular total de páginas
- Pasar información a la vista
- Generar controles de paginación

### 3. Caché

**¿Qué cachear?**
- Datos que no cambian frecuentemente
- Listas de referencia (empresas, lugares)
- Configuraciones del sistema

**IMemoryCache:**
- Servicio de caché en memoria
- Configurar tiempo de expiración
- Sliding expiration vs absolute expiration

**Implementación:**
- Inyectar IMemoryCache en controlador
- Verificar si existe en caché
- Si no existe, consultar BD y guardar en caché
- Retornar datos desde caché

**Ejemplo:**
- Cachear lista de empresas activas por 5 minutos
- Evitar consultar BD en cada petición

### 4. AsNoTracking para Consultas de Solo Lectura

**Cuándo usar:**
- Listados que no se modificarán
- Consultas de solo lectura
- Reportes y estadísticas

**Ventajas:**
- Mejor rendimiento
- Menor uso de memoria
- No rastrea cambios

**Implementación:**
- Agregar .AsNoTracking() a la consulta
- Antes de ToListAsync()

### 5. Proyecciones para Reducir Datos

**Problema:**
- Cargar entidades completas cuando solo se necesitan algunos campos
- Transferir datos innecesarios

**Solución:**
- Usar Select() para proyectar solo lo necesario
- Crear ViewModels con solo los campos requeridos

**Ventajas:**
- Menos datos transferidos
- Consultas SQL más eficientes
- Mejor rendimiento

---

## Buenas Prácticas

### 1. Patrón Repository (Opcional)

**Propósito:**
- Abstraer el acceso a datos
- Facilitar testing
- Centralizar lógica de consultas

**Estructura:**
- Interfaz IRepository con métodos comunes
- Implementación concreta por entidad
- Inyección de dependencias

**Ventajas:**
- Código más testeable
- Separación de responsabilidades
- Reutilización de código

**Cuándo usar:**
- Proyectos grandes
- Equipos grandes
- Necesidad de testing extensivo

### 2. ViewModels para Vistas

**Propósito:**
- Separar modelos de dominio de modelos de vista
- Incluir solo datos necesarios
- Agregar datos adicionales para la vista

**Ejemplo:**
- EmpleadoFormViewModel para formularios
- Incluye SelectList de empresas
- Solo propiedades necesarias

**Ventajas:**
- Vistas más limpias
- Mejor separación de responsabilidades
- Más fácil de mantener

### 3. Manejo Centralizado de Errores

**Middleware personalizado:**
- Captura todas las excepciones no manejadas
- Registra en log
- Redirige a página de error amigable

**Configuración:**
- Crear clase de middleware
- Registrar en Program.cs
- Configurar página de error

**Ventajas:**
- Errores consistentes
- Mejor experiencia de usuario
- Facilita debugging

### 4. Logging

**ILogger:**
- Servicio de logging integrado
- Diferentes niveles (Debug, Information, Warning, Error)
- Múltiples proveedores (consola, archivo, etc.)

**Uso:**
- Inyectar ILogger<T> en controladores
- Registrar operaciones importantes
- Registrar errores con contexto

**Niveles:**
- Debug: Información detallada para desarrollo
- Information: Eventos importantes
- Warning: Situaciones anormales pero manejables
- Error: Errores que requieren atención

---

## Preparación para Producción

### 1. Configuración de Entornos

**appsettings.Development.json:**
- Configuración para desarrollo
- Cadena de conexión local
- Logging detallado
- Errores detallados habilitados

**appsettings.Production.json:**
- Configuración para producción
- Cadena de conexión del servidor
- Logging mínimo
- Errores genéricos

**Variables de entorno:**
- Configurar según el entorno
- Usar IWebHostEnvironment para detectar

### 2. Migraciones en Producción

**Opciones:**

**Opción 1: Aplicar automáticamente al iniciar:**
- Llamar a context.Database.Migrate() en Program.cs
- Ventaja: Automático
- Desventaja: Puede causar problemas si falla

**Opción 2: Script SQL:**
- Generar script con dotnet ef migrations script
- Ejecutar manualmente en producción
- Ventaja: Control total
- Desventaja: Proceso manual

**Recomendación:**
- Opción 2 para producción
- Opción 1 para desarrollo/staging

### 3. Seguridad

**HTTPS:**
- Habilitar UseHttpsRedirection()
- Usar certificado SSL válido
- Configurar HSTS

**Políticas de seguridad:**
- Configurar antiforgery tokens
- Validar todos los inputs
- Sanitizar datos de usuario

**Secretos:**
- No guardar contraseñas en código
- Usar User Secrets en desarrollo
- Usar Azure Key Vault o similar en producción

### 4. Optimización de Assets

**CSS y JavaScript:**
- Minificar archivos
- Combinar archivos
- Usar CDN para librerías

**Imágenes:**
- Optimizar tamaño
- Usar formatos modernos (WebP)
- Lazy loading

**Caché del navegador:**
- Configurar headers de caché
- Versionar archivos estáticos

---

## Despliegue

### Opciones de Hosting

**1. IIS (Internet Information Services):**
- Servidor web de Microsoft
- Integración con Windows Server
- Configuración familiar

**2. Azure App Service:**
- PaaS de Microsoft
- Escalado automático
- Integración con Azure SQL

**3. Docker:**
- Contenedores
- Portabilidad
- Fácil escalado

### Proceso de Despliegue

**Preparación:**
1. Configurar appsettings.Production.json
2. Generar build de producción
3. Ejecutar tests
4. Generar scripts de migración

**Publicación:**
1. Publicar aplicación
2. Copiar archivos al servidor
3. Aplicar migraciones
4. Configurar IIS/servidor web
5. Verificar funcionamiento

**Post-despliegue:**
1. Verificar que todo funcione
2. Monitorear logs
3. Realizar pruebas de humo
4. Notificar a usuarios

---

## Resumen de la Migración Completa

### Checklist de Migración

**Unidad 1: Fundamentos**
- [x] Crear proyecto ASP.NET Core MVC
- [x] Configurar estructura de carpetas
- [x] Entender el patrón MVC

**Unidad 2: CRUD**
- [x] Migrar modelos de dominio
- [x] Implementar CRUD de Empresas
- [x] Implementar CRUD de Empleados
- [x] Implementar CRUD de Lugares

**Unidad 3: Entity Framework**
- [x] Configurar DbContext
- [x] Crear migraciones iniciales
- [x] Migrar procedimientos almacenados
- [x] Implementar consultas LINQ

**Unidad 4: Identity**
- [x] Configurar ASP.NET Identity
- [x] Crear roles (Admin, Supervisor, Cocina)
- [x] Implementar login/logout
- [x] Proteger controladores con [Authorize]

**Unidad 5: Validaciones y Reportes**
- [x] Implementar validaciones
- [x] Generar reportes PDF
- [x] Exportar a Excel (opcional)

**Unidad 6: Avanzado**
- [x] Implementar AJAX
- [x] Configurar SignalR (opcional)
- [x] Optimizar consultas
- [x] Preparar para producción

### Funcionalidades Migradas

✅ **Gestión de Empresas**
✅ **Gestión de Empleados**
✅ **Gestión de Lugares**
✅ **Gestión de Servicios**
✅ **Registro de Comensales**
✅ **Registro Manual**
✅ **Reportes**
✅ **Estadísticas**
✅ **Configuración del Sistema**
✅ **Autenticación y Autorización**

---

## ¡Felicitaciones!

Has completado la guía de migración de tu **Sistema Control de Almuerzos** de Windows Forms a ASP.NET MVC.

### Tu aplicación ahora:

✅ Es accesible desde cualquier navegador
✅ Tiene autenticación y autorización
✅ Usa Entity Framework para acceso a datos
✅ Genera reportes PDF
✅ Tiene validaciones robustas
✅ Está optimizada para producción
✅ Soporta múltiples usuarios simultáneos
✅ Tiene actualización en tiempo real (opcional)

--
