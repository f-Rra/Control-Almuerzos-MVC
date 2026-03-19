
<div align="center">
  <img src="./SCA-MVC/wwwroot/images/logo.png" alt="Logo SCA" width="120"/>
  <h1>Sistema de Control de Almuerzos — MVC</h1>
</div>

---

Migración completa del [Sistema de Control de Almuerzos (WinForms)](https://github.com/f-Rra/Sistema-Control-Almuerzos) a **ASP.NET Core MVC (.NET 9)**, manteniendo todas las funcionalidades del sistema original y agregando nuevas capacidades propias de una aplicación web: autenticación con Identity, roles y permisos, envío de reportes por email, diseño glassmorphism responsivo y gestión de usuarios.

![.NET 9](https://img.shields.io/badge/.NET-9.0-blue)
![C#](https://img.shields.io/badge/C%23-13.0-green)
![EF Core](https://img.shields.io/badge/EF%20Core-9.0-purple)
![SQL Server](https://img.shields.io/badge/SQL%20Server-2019+-red)
![ASP.NET MVC](https://img.shields.io/badge/UI-ASP.NET%20MVC-lightblue)
![Identity](https://img.shields.io/badge/Auth-Identity-orange)
![Status](https://img.shields.io/badge/Status-Production%20Ready-brightgreen)

---

## Características Principales

- **Arquitectura MVC con inyección de dependencias** (Controllers → Services → EF Core → SQL Server)
- **Migración de ADO.NET a Entity Framework Core** con LINQ y Fluent API
- **ASP.NET Core Identity** con roles (Admin / Usuario) y sidebar adaptativo
- **Registro rápido de comensales** por ID de credencial (preparado para RFID)
- **Gestión completa de empleados y empresas** con asignación de credenciales
- **Sistema de servicios por jornada** (comedor y quincho)
- **Cronómetro en tiempo real** con barra de progreso y notificaciones toast
- **Reportes automáticos** con exportación a PDF (QuestPDF) y envío por email (MailKit)
- **Estadísticas avanzadas** por empresa, período y lugar
- **4 tipos de reporte**: Servicios, Asistencias por Empresa, Cobertura vs Proyección, Distribución por Día
- **Dashboard** con resumen de últimos 30 días y comparativa semanal
- **Registro manual alternativo** con selección múltiple
- **Manejo de invitados** sin datos personales
- **Gestión de usuarios del sistema** (crear, editar, activar/desactivar, asignar roles)
- **Middleware global** de manejo de excepciones (404, 500)
- **Interfaz moderna** con diseño glassmorphism y tipografía Outfit
- **Validaciones robustas** (duplicados, servicio activo, estado de empleado, credencial única)
- **Soft-delete** en todas las entidades (baja lógica)

---

## Funcionalidades del Sistema

### Dashboard (Página de Inicio)

**Lista de Últimos Servicios:**
- Visualización de los servicios más recientes (últimos 30 días)
- Ordenados cronológicamente (más recientes primero)
- Información resumida: fecha, lugar, proyección, duración

**Detalles del Servicio Seleccionado:**
- Información completa del servicio seleccionado
- Fecha y hora de inicio, lugar, proyección inicial
- Total de invitados, duración del servicio
- Total de comensales registrados
- Comparativa final proyección vs real

**Comparativa Semanal:**
- Gráfico de barras lunes a viernes
- Tendencias de asistencia por día de la semana

---

### Gestión de Servicios

**Configuración e Inicio:**
- Selección de lugar (Comedor/Quincho)
- Ingreso de proyección de comensales esperados
- Registro de total de invitados estimados
- Validación de datos antes de activar

**Panel Informativo Durante el Servicio:**
- Lugar actual del servicio activo
- Cronómetro de duración en tiempo real (HH:mm:ss)
- Barra de progreso de cobertura (registrados vs proyección)
- Contador de comensales registrados (actualización automática)
- Indicadores de registrados y faltantes

**Registro por Credencial:**
- Campo de ingreso para ID de credencial
- Validación inmediata al ingresar ID
- Notificación glassmorphism temporal (nombre, empresa, hora)
- Listado de comensales registrados en el servicio actual

**Finalización del Servicio:**
- Cierre del servicio activo con cálculo automático de estadísticas
- Auto-cierre preventivo de servicios abandonados (`FinalizarPendientesAsync`)

**Validaciones Automáticas:**
- Verificación de empleado activo en el sistema
- Detección de registros duplicados en el servicio actual
- No se puede iniciar un servicio si ya hay uno activo
- Proyección y invitados validados en rango

---

### Registro Manual de Comensales

**Método Alternativo de Registro:**
- Filtro por empresa (combo desplegable)
- Búsqueda por nombre del empleado
- Tabla de empleados pendientes con checkboxes
- Registro masivo vía AJAX (selección múltiple)
- Contadores de registrados vs pendientes en tiempo real
- Mismas validaciones que el sistema por credencial

---

### Reportes

- **Selección de tipo de reporte**: 4 tipos disponibles
- **Filtros personalizables**: Rango de fechas y filtro por lugar
- **Exportar a PDF**: Documento estilizado con QuestPDF (header, tablas formateadas, paginación)
- **Enviar por Email**: Adjuntar PDF generado y enviarlo vía SMTP con MailKit

**Tipos de Reportes Disponibles:**

**1. Lista de Servicios**
- Todos los servicios del período seleccionado
- Fecha, lugar, proyección, duración, total real vs proyectado

**2. Asistencias por Empresa**
- Total de asistencias por cada compañía del predio
- Comparativa y ranking entre empresas

**3. Cobertura vs Proyección**
- Comparación entre proyección inicial y asistencia real
- Porcentaje de cobertura por servicio

**4. Distribución por Día de Semana**
- Patrones de asistencia semanal
- Total acumulado por cada día de la semana

---

### Panel de Administración (Solo Admin)

Punto de acceso centralizado a todas las funciones administrativas:

- **Empresas**: Gestión completa de empresas del predio
- **Empleados**: Administración de empleados y credenciales
- **Estadísticas**: Dashboard de análisis y métricas del sistema
- **Usuarios**: Gestión de cuentas, roles y accesos

---

### Gestión de Empresas (Solo Admin)

**Operaciones ABM Completas:**
- **Alta**: Crear nuevas empresas
- **Baja lógica**: Desactivar empresas manteniendo historial
- **Modificación**: Actualizar información de empresas existentes
- **Listado y Búsqueda**: Filtros por nombre con conteo

**Visualización de Estadísticas:**
- Total de empleados activos e inactivos
- Total de asistencias del mes actual
- Promedio diario de asistencias

---

### Gestión de Empleados (Solo Admin)

**Operaciones ABM Completas:**
- **Alta**: Crear empleados con credencial, nombre, apellido y empresa
- **Baja lógica**: Desactivar empleados manteniendo historial
- **Modificación**: Actualizar información y reasignar empresa
- **Verificación de Credencial**: Validación AJAX de unicidad

**Gestión de Credenciales:**
- Asignación de ID de credencial RFID a empleados
- Validación de unicidad de credenciales (regex + server-side)
- Visualización de estado de credencial (disponible/en uso)

---

### Estadísticas

Dashboard de análisis estadístico con KPIs organizados en secciones:

- **Empleados**: Total, activos, inactivos
- **Empresas**: Activas, con empleados, promedio por empresa
- **Servicios**: Total del mes, del año, promedio diario
- **Asistencias**: Total, empleados vs invitados, cobertura, duración promedio
- **Top 5 Empresas**: Ranking con barras de progreso y porcentajes

---

### Gestión de Usuarios (Solo Admin)

- Listado de usuarios del sistema con sus roles
- Creación de nuevos usuarios con asignación de rol (Admin / Usuario)
- Edición de usuario (cambiar rol, resetear contraseña)
- Activación/desactivación vía `LockoutEnd` (sin eliminar el registro)
- Badges visuales de rol (Admin en dorado, Usuario en azul)

---

## Arquitectura del Sistema

```
Control-Almuerzos-MVC/
├── SCA-MVC/
│   ├── Controllers/                  # Controladores MVC (10 controllers)
│   │   ├── HomeController            # Dashboard principal
│   │   ├── ServicioController        # Gestión de servicios de almuerzo
│   │   ├── RegistroController        # Registro manual de comensales
│   │   ├── ReporteController         # Reportes, PDF y envío por email
│   │   ├── EmpresaController         # ABM de empresas
│   │   ├── EmpleadoController        # ABM de empleados
│   │   ├── EstadisticaController     # KPIs y estadísticas
│   │   ├── AdminController           # Panel de administración
│   │   ├── UsuarioController         # Gestión de usuarios del sistema
│   │   └── AccountController         # Login, registro, logout
│   │
│   ├── Models/                       # Entidades del dominio
│   │   ├── Empleado.cs               # Modelo de empleados
│   │   ├── Empresa.cs                # Modelo de empresas
│   │   ├── Lugar.cs                  # Modelo de lugares (comedor/quincho)
│   │   ├── Servicio.cs               # Modelo de servicios por jornada
│   │   ├── Registro.cs               # Modelo de registros de almuerzos
│   │   ├── ApplicationUser.cs        # Modelo extendido de Identity
│   │   └── ErrorViewModel.cs         # Modelo para páginas de error
│   │
│   ├── ViewModels/                   # Modelos de vista para cada módulo
│   │   ├── DashboardViewModel.cs     # VM del dashboard principal
│   │   ├── ServicioActivoViewModel.cs# VM del servicio activo
│   │   ├── EmpresaViewModel.cs       # VM de empresas (lista + form)
│   │   ├── EmpleadoViewModel.cs      # VM de empleados (lista + form)
│   │   ├── ReporteViewModel.cs       # VM de reportes (filtros + datos)
│   │   ├── EstadisticasViewModel.cs  # VM de estadísticas (KPIs)
│   │   ├── UsuarioViewModel.cs       # VM de usuarios (lista + form)
│   │   └── AccountViewModels.cs      # VM de login y registro
│   │
│   ├── Views/                        # Vistas Razor organizadas por controlador
│   │   ├── Home/                     # Dashboard + NotFound
│   │   ├── Servicio/                 # Gestión de servicios
│   │   ├── Registro/                 # Registro manual
│   │   ├── Reporte/                  # Reportes y filtros
│   │   ├── Empresa/                  # ABM de empresas
│   │   ├── Empleado/                 # ABM de empleados
│   │   ├── Estadistica/             # KPIs y estadísticas
│   │   ├── Admin/                    # Panel de administración
│   │   ├── Usuario/                  # Gestión de usuarios
│   │   ├── Account/                  # Login, registro, acceso denegado
│   │   └── Shared/                   # Layout, partials, error pages
│   │       ├── _Layout.cshtml        # Layout principal (sidebar + topbar)
│   │       ├── _ServicioCard.cshtml  # Partial: card de servicio
│   │       ├── _EmpleadoRow.cshtml   # Partial: fila de empleado
│   │       ├── _KpiCard.cshtml       # Partial: card de KPI
│   │       ├── _FiltroFechas.cshtml  # Partial: filtro de fechas
│   │       ├── _Paginacion.cshtml    # Partial: controles de paginación
│   │       ├── _Notificaciones.cshtml# Partial: toasts de TempData
│   │       ├── Error.cshtml          # Página de error 500
│   │       └── NotFound.cshtml       # Página de error 404
│   │
│   ├── Services/                     # Capa de negocio (interfaces + impl.)
│   │   ├── IEmpresaNegocio.cs        # Interfaz de empresas
│   │   ├── EmpresaNegocio.cs         # Implementación con EF Core
│   │   ├── IEmpleadoNegocio.cs       # Interfaz de empleados
│   │   ├── EmpleadoNegocio.cs        # Implementación con EF Core
│   │   ├── IServicioNegocio.cs       # Interfaz de servicios
│   │   ├── ServicioNegocio.cs        # Implementación con EF Core
│   │   ├── IRegistroNegocio.cs       # Interfaz de registros
│   │   ├── RegistroNegocio.cs        # Implementación con EF Core
│   │   ├── ILugarNegocio.cs          # Interfaz de lugares
│   │   ├── LugarNegocio.cs           # Implementación con EF Core
│   │   ├── IReporteNegocio.cs        # Interfaz de reportes
│   │   ├── ReporteNegocio.cs         # Implementación con EF Core
│   │   ├── IEstadisticasNegocio.cs   # Interfaz de estadísticas
│   │   ├── EstadisticasNegocio.cs    # Implementación con EF Core
│   │   ├── IEmailService.cs          # Interfaz de envío de email
│   │   └── EmailService.cs           # Implementación con MailKit
│   │
│   ├── Data/                         # Contexto y configuraciones EF
│   │   ├── ApplicationDbContext.cs   # DbContext + Seeding
│   │   └── Configurations/           # IEntityTypeConfiguration<T>
│   │       ├── EmpresaConfiguration.cs
│   │       ├── EmpleadoConfiguration.cs
│   │       ├── LugarConfiguration.cs
│   │       ├── ServicioConfiguration.cs
│   │       └── RegistroConfiguration.cs
│   │
│   ├── Helpers/                      # Utilidades
│   │   ├── MensajesConstantes.cs     # Constantes de mensajes en español
│   │   ├── MensajesUI.cs             # Extensiones de TempData para toasts
│   │   ├── SpanishIdentityErrorDescriber.cs  # Errores de Identity en español
│   │   └── AppUserClaimsPrincipalFactory.cs  # Claims personalizados
│   │
│   ├── Middleware/                   # Pipeline personalizado
│   │   └── ExceptionMiddleware.cs    # Manejo global de excepciones
│   │
│   ├── Migrations/                   # Migraciones de Entity Framework
│   ├── SQL/                          # Scripts SQL originales
│   │   └── Procedimientos_Vistas_Triggers.sql
│   │
│   ├── wwwroot/                      # Archivos estáticos
│   │   ├── css/site.css              # Estilos globales (glassmorphism)
│   │   ├── js/site.js                # Modales, notificaciones, confirmaciones
│   │   ├── images/                   # Iconos de navegación y logo
│   │   └── lib/                      # Bootstrap (distribución local)
│   │
│   ├── Program.cs                    # Punto de entrada, DI, Identity, Seeding
│   ├── appsettings.json              # Configuración (DB, Email, Logging)
│   └── SCA-MVC.csproj                # Paquetes NuGet del proyecto
│
├── Sistema-Control-Almuerzos/        # Proyecto original WinForms (referencia)
├── Guias/                            # Documentación de implementación por commit
└── README.md                         # Este archivo
```

---

## Base de Datos

### Modelo de Datos

**EMPLEADOS**
```sql
- IdEmpleado (INT, PK, Identity)
- Nombre (VARCHAR(50), NOT NULL)
- Apellido (VARCHAR(50), NOT NULL)
- IdEmpresa (INT, FK, NOT NULL)
- IdCredencial (VARCHAR(20), UNIQUE)
- FotoUrl (VARCHAR(200))
- Estado (BIT, DEFAULT 1)
```

**EMPRESAS**
```sql
- IdEmpresa (INT, PK, Identity)
- Nombre (VARCHAR(100), NOT NULL, UNIQUE)
- Estado (BIT, DEFAULT 1)
```

**LUGARES**
```sql
- IdLugar (INT, PK, Identity)
- Nombre (VARCHAR(50), NOT NULL)
- Descripcion (VARCHAR(200))
- Estado (BIT, DEFAULT 1)
```

**SERVICIOS**
```sql
- IdServicio (INT, PK, Identity)
- IdLugar (INT, FK, NOT NULL)
- Fecha (DATE, NOT NULL)
- Proyeccion (INT)
- DuracionMinutos (INT)
- TotalComensales (INT, DEFAULT 0)
- TotalInvitados (INT, DEFAULT 0)
- Estado (VARCHAR(20), DEFAULT 'Activo')
```

**REGISTROS**
```sql
- IdRegistro (INT, PK, Identity)
- IdEmpleado (INT, FK, NOT NULL)
- IdEmpresa (INT, FK, NOT NULL)
- IdServicio (INT, FK, NOT NULL)
- Fecha (DATE, NOT NULL)
- Hora (TIME, NOT NULL)
- IdLugar (INT, FK, NOT NULL)
```

### Configuraciones Fluent API

Cada entidad tiene su propia clase de configuración (`IEntityTypeConfiguration<T>`) en `Data/Configurations/`:

- Relaciones `HasOne/WithMany` con `OnDelete(Restrict)`
- Índices únicos (`HasIndex().IsUnique()`)
- Check constraints (`HasCheckConstraint`)
- Valores por defecto (`HasDefaultValue`)
- Seeding de datos iniciales (`HasData`)

### Datos Iniciales (Seeding)

Al aplicar las migraciones, se crean automáticamente:
- **2 Lugares**: Comedor, Quincho
- **12 Empresas** del complejo industrial
- **60 Empleados** con credenciales RF001 – RF060

---

## Herramientas y Tecnologías Utilizadas

### Desarrollo del Sistema

**IDE y Entorno de Desarrollo:**
- **Visual Studio 2022 Community Edition** — Desarrollo de la aplicación ASP.NET Core MVC
- **SQL Server Management Studio (SSMS) 19** — Gestión de base de datos

**Frameworks y Librerías:**

| Paquete | Versión | Uso |
|---|---|---|
| **.NET 9** | 9.0 | Framework principal de la aplicación |
| **Entity Framework Core** | 9.0.0 | ORM — consultas LINQ, migraciones, Fluent API |
| **ASP.NET Core Identity** | 9.0.0 | Autenticación, autorización y roles |
| **QuestPDF** | 2026.2.1 | Generación de PDFs estilizados |
| **MailKit / MimeKit** | 4.15.1 | Envío de emails vía SMTP |
| **Bootstrap Icons** | 1.11.2 | Iconografía del sistema |
| **Google Fonts (Outfit)** | — | Tipografía principal |

**Base de Datos:**
- **SQL Server 2019+ Express Edition** — Motor de base de datos
- **Entity Framework Core Migrations** — Gestión de esquema

**Control de Versiones:**
- **Git** — Control de versiones local
- **GitHub** — Repositorio remoto

---

## Características Técnicas

### Seguridad

- **ASP.NET Core Identity**: Autenticación completa con hash de contraseñas
- **Roles**: `Admin` (acceso total) y `Usuario` (solo operativo)
- **[Authorize]**: En todos los controladores
- **[ValidateAntiForgeryToken]**: En todos los POST
- **Middleware global**: Captura de excepciones no controladas (404, 500)
- **Baja lógica**: No se eliminan datos, solo se desactivan
- **Integridad referencial**: Foreign Keys con `OnDelete(Restrict)`

### Rendimiento

- **Entity Framework Core**: Consultas LINQ optimizadas con `.Include()`, `.AsNoTracking()`
- **Inyección de dependencias**: Servicios `Scoped` para ciclo de vida por request
- **Vistas parciales**: Componentes reutilizables (`_ServicioCard`, `_KpiCard`, etc.)
- **Fluent API separada**: Configuraciones en clases independientes por entidad
- **AJAX**: Registro de comensales y filtros sin recargas de página
- **Auto-cierre preventivo**: `FinalizarPendientesAsync()` evita servicios abandonados

### Validaciones Implementadas

**A Nivel de Base de Datos:**
- Unicidad de credenciales (Unique Index)
- Integridad referencial (FK Constraints)
- Check constraints en campos numéricos
- Valores por defecto en estados

**A Nivel de Negocio (Services):**
- Empleado debe estar activo
- No puede registrarse dos veces en el mismo servicio
- Debe existir un servicio activo para registrar
- Credencial debe ser única al asignar
- No se puede cerrar un servicio sin inicio
- Proyección y invitados en rango válido

**A Nivel de Presentación:**
- Validaciones client-side con jQuery Validation
- Mensajería centralizada con constantes (`MensajesConstantes`)
- Confirmaciones glassmorphism antes de operaciones críticas
- Feedback visual inmediato con toasts (`MensajesUI`)
- Errores de Identity traducidos al español (`SpanishIdentityErrorDescriber`)

---

## Requisitos Previos

- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- [SQL Server Express o Developer](https://www.microsoft.com/es-es/sql-server/sql-server-downloads)
- [Visual Studio 2022](https://visualstudio.microsoft.com/) (recomendado) o VS Code
- (Opcional) Cuenta Gmail con [Contraseña de Aplicación](https://myaccount.google.com/apppasswords) para envío de emails

---

## Instalación y Configuración

### 1. Clonar el repositorio
```bash
git clone https://github.com/f-Rra/Control-Almuerzos-MVC.git
cd Control-Almuerzos-MVC/SCA-MVC
```

### 2. Configurar la cadena de conexión

Editar `appsettings.json` y reemplazar el `Server` con tu instancia de SQL Server:

```json
"ConnectionStrings": {
    "DefaultConnection": "Server=TU-SERVIDOR\\SQLEXPRESS;Database=BD_Control_Almuerzos;Integrated Security=true;TrustServerCertificate=true;MultipleActiveResultSets=true;"
}
```

### 3. Aplicar migraciones

Desde la **Consola del Administrador de Paquetes** en Visual Studio:
```
Update-Database
```

O desde la terminal:
```bash
dotnet ef database update
```

> Las migraciones crean automáticamente la base de datos, las tablas, los índices, las restricciones y los datos iniciales (Seeding): 2 Lugares, 12 Empresas y 60 Empleados con credenciales RFID.

### 4. (Opcional) Configurar envío de email

Para habilitar el envío de reportes por correo, editar la sección `EmailSettings` en `appsettings.json`:

```json
"EmailSettings": {
    "SmtpHost": "smtp.gmail.com",
    "SmtpPort": "587",
    "SmtpUser": "tu-correo@gmail.com",
    "SmtpPass": "xxxx xxxx xxxx xxxx",
    "FromEmail": "tu-correo@gmail.com",
    "FromName": "Sistema Control de Almuerzos"
}
```

### 5. Ejecutar la aplicación

```bash
dotnet run
```

O presionar `F5` en Visual Studio.

---

## Usuario por Defecto

Al iniciar la aplicación por primera vez, el seeding crea automáticamente un usuario administrador:

| Campo | Valor |
|---|---|
| **Email** | `admin@sca.com` |
| **Contraseña** | `Admin123` |
| **Rol** | Admin |

> ⚠️ Se recomienda cambiar la contraseña del administrador tras el primer inicio de sesión.

---

## Documentación

### Documentos Disponibles

| Documento | Descripción | Ubicación |
|---|---|---|
| **README.md** | Documentación técnica completa (este archivo) | Raíz del proyecto |
| **Guias/** | Documentación de implementación por commit | `Guias/` |
| **Guia_Implementacion_Completa.md** | Hoja de ruta completa de todos los commits | `Guias/` |
| **Procedimientos_Vistas_Triggers.sql** | Script SQL original (referencia) | `SCA-MVC/SQL/` |

---

## Herramientas de Documentación y Asistencia

Las siguientes herramientas fueron utilizadas para la elaboración de documentación técnica, guías de usuario y asistencia en la estructuración del código:

- **GitHub Copilot** (Claude Sonnet 4.5)
  - Generación de documentación técnica (README.md)
  - Asistencia en refactorización de código
  - Sugerencias de mejores prácticas
  - Revisión de consultas LINQ y configuraciones EF
  - Generación de guías de implementación por commit

### Nota sobre el uso de IA

El uso de herramientas de IA generativa fue exclusivamente para:
- **Documentación**: Redacción clara y profesional de guías
- **Refactorización**: Mejora de estructura y legibilidad del código existente
- **Consultoría**: Validación de soluciones técnicas y mejores prácticas
- **Patrones de diseño**: Sugerencias para organización de código (Services, Helpers, Configurations)

**Toda la lógica de negocio, arquitectura del sistema, diseño de base de datos y funcionalidades fueron desarrolladas por el autor del proyecto.**

---

## Enlaces Útiles

- [Repositorio MVC en GitHub](https://github.com/f-Rra/Control-Almuerzos-MVC)
- [Proyecto Original WinForms](https://github.com/f-Rra/Sistema-Control-Almuerzos)
- [Documentación de .NET 9](https://learn.microsoft.com/en-us/dotnet/core/whats-new/dotnet-9)
- [Entity Framework Core](https://learn.microsoft.com/en-us/ef/core/)
- [ASP.NET Core Identity](https://learn.microsoft.com/en-us/aspnet/core/security/authentication/identity)
- [QuestPDF](https://www.questpdf.com/)
- [MailKit](https://github.com/jstedfast/MailKit)

---

**Facundo Herrera**
- Estudiante de Tecnicatura Universitaria en Programación
- Universidad Tecnológica Nacional — Facultad Regional General Pacheco (UTN-FRGP)
- Email: Facundo.herrera@alumnos.frgp.utn.edu.ar

---