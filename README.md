# Sistema Control de Almuerzos — MVC

Rediseño web del [Sistema Control de Almuerzos](https://github.com/f-Rra/Sistema-Control-Almuerzos) original (WinForms, C#) sobre **ASP.NET Core MVC**: autenticación con Identity, roles y permisos, registro de comensales por credencial RFID, reportes PDF, envío por email y gestión completa de empleados y usuarios.

![.NET 9](https://img.shields.io/badge/.NET-9.0-blue)
![C#](https://img.shields.io/badge/C%23-13.0-green)
![EF Core](https://img.shields.io/badge/EF%20Core-9.0-purple)
![SQL Server](https://img.shields.io/badge/SQL%20Server-2019+-red)
![ASP.NET MVC](https://img.shields.io/badge/UI-ASP.NET%20MVC-lightblue)
![Identity](https://img.shields.io/badge/Auth-Identity-orange)
![Status](https://img.shields.io/badge/Status-Demo%20Ready-yellow)

---

## Funcionalidades del Sistema

### Dashboard

**Lista de Últimos Servicios:**
- Visualización de los servicios más recientes (últimos 30 días)
- Ordenados cronológicamente (más recientes primero)
- Información resumida: fecha, lugar, proyección, duración

**Detalles del Servicio Seleccionado:**
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

**Registro por Credencial RFID:**
- Campo de ingreso para ID de credencial
- Validación inmediata al ingresar ID
- Notificación temporal (nombre, empresa, hora)
- Listado de comensales registrados en el servicio actual
- Feedback visual táctil: flash verde en nueva fila, animación de contadores

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

- Filtro por empresa (combo desplegable)
- Búsqueda por nombre del empleado
- Tabla de empleados pendientes con checkboxes
- Registro masivo vía AJAX (selección múltiple)
- Contadores de registrados vs pendientes en tiempo real
- Mismas validaciones que el sistema por credencial

---

### Reportes

- **Filtros personalizables**: Rango de fechas y filtro por lugar
- **Exportar a PDF**: Documento estilizado con QuestPDF (header, tablas formateadas, paginación)
- **Enviar por Email**: Adjuntar PDF generado y enviarlo vía SMTP con MailKit

**Tipos de Reportes Disponibles:**

| # | Tipo | Descripción |
|---|---|---|
| 1 | Lista de Servicios | Todos los servicios del período: fecha, lugar, proyección, duración, real vs proyectado |
| 2 | Asistencias por Empresa | Total de asistencias por compañía, comparativa y ranking |
| 3 | Cobertura vs Proyección | Comparación proyección inicial vs asistencia real, porcentaje por servicio |
| 4 | Distribución por Día de Semana | Patrones de asistencia, total acumulado por día |

---

### Panel de Administración

Punto de acceso centralizado con métricas reales del sistema (empresas, empleados, asistencias del día, usuarios):

- **Empresas**: Gestión completa de empresas del predio
- **Empleados**: Administración de empleados y credenciales RFID
- **Estadísticas**: Dashboard de análisis y métricas del sistema
- **Usuarios**: Gestión de cuentas, roles y accesos

---

### Gestión de Empresas

**Operaciones ABM Completas:**
- Alta / Baja lógica / Modificación
- Búsqueda y filtrado por nombre

**Estadísticas por empresa:**
- Total de empleados activos e inactivos
- Total de asistencias del mes actual
- Promedio diario de asistencias

---

### Gestión de Empleados

**Operaciones ABM Completas:**
- Alta: Crear empleados con credencial RFID, nombre, apellido y empresa
- Baja lógica: Desactivar empleados manteniendo historial
- Modificación: Actualizar información y reasignar empresa
- Verificación AJAX de unicidad de credencial

---

### Estadísticas

Dashboard de análisis con KPIs organizados en secciones:

- **Empleados**: Total, activos, inactivos
- **Empresas**: Activas, con empleados, promedio por empresa
- **Servicios**: Total del mes, del año, promedio diario
- **Asistencias**: Total, empleados vs invitados, cobertura, duración promedio
- **Top 5 Empresas**: Ranking con barras de progreso y porcentajes

---

### Gestión de Usuarios

- Listado de usuarios del sistema con sus roles
- Creación de nuevos usuarios con asignación de rol (Admin / Usuario)
- Edición de usuario (cambiar rol, resetear contraseña)
- Activación/desactivación vía `LockoutEnd` (sin eliminar el registro)

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
│   │   └── AccountController         # Login, logout, acceso denegado
│   │
│   ├── Models/                       # Entidades del dominio
│   │   ├── Empleado.cs
│   │   ├── Empresa.cs
│   │   ├── Lugar.cs
│   │   ├── Servicio.cs
│   │   ├── Registro.cs
│   │   ├── ApplicationUser.cs        # Modelo extendido de Identity
│   │   └── ErrorViewModel.cs
│   │
│   ├── ViewModels/                   # Modelos de vista para cada módulo
│   │   ├── DashboardViewModel.cs
│   │   ├── ServicioActivoViewModel.cs
│   │   ├── EmpresaViewModel.cs
│   │   ├── EmpleadoViewModel.cs
│   │   ├── ReporteViewModel.cs
│   │   ├── EstadisticasViewModel.cs
│   │   ├── AdminViewModel.cs
│   │   ├── UsuarioViewModel.cs
│   │   └── AccountViewModels.cs
│   │
│   ├── Views/                        # Vistas Razor organizadas por controlador
│   │   ├── Shared/
│   │   │   ├── _Layout.cshtml        # Layout principal (sidebar + topbar + reloj)
│   │   │   ├── _ServicioCard.cshtml
│   │   │   ├── _EmpleadoRow.cshtml
│   │   │   ├── _KpiCard.cshtml
│   │   │   ├── _FiltroFechas.cshtml
│   │   │   ├── _Paginacion.cshtml
│   │   │   ├── _Notificaciones.cshtml
│   │   │   ├── Error.cshtml          # Página de error 500
│   │   │   └── NotFound.cshtml       # Página de error 404
│   │   └── [Home|Servicio|Registro|Reporte|Empresa|Empleado|Estadistica|Admin|Usuario|Account]/
│   │
│   ├── Services/                     # Capa de negocio (interfaces + implementaciones)
│   │   ├── I*Negocio.cs              # Interfaces
│   │   ├── *Negocio.cs               # Implementaciones con EF Core
│   │   ├── IEmailService.cs
│   │   └── EmailService.cs           # MailKit / MimeKit
│   │
│   ├── Data/
│   │   ├── ApplicationDbContext.cs   # DbContext + Seeding
│   │   └── Configurations/           # IEntityTypeConfiguration<T> por entidad
│   │
│   ├── Helpers/
│   │   ├── MensajesConstantes.cs
│   │   ├── MensajesUI.cs             # Extensiones de TempData para toasts
│   │   ├── SpanishIdentityErrorDescriber.cs
│   │   └── AppUserClaimsPrincipalFactory.cs
│   │
│   ├── Middleware/
│   │   └── ExceptionMiddleware.cs    # Manejo global de excepciones (404, 500)
│   │
│   ├── Migrations/
│   ├── SQL/
│   │   └── Procedimientos_Vistas_Triggers.sql
│   │
│   ├── wwwroot/
│   │   ├── css/site.css
│   │   ├── js/site.js
│   │   ├── images/logo.png
│   │   └── lib/                      # Bootstrap 5 (distribución local)
│   │
│   ├── Program.cs                    # Punto de entrada, DI, Identity, Seeding
│   ├── appsettings.json
│   └── SCA-MVC.csproj
│
├── Guias/                            # 51 guías documentando el proceso commit a commit
└── README.md
```

---

## Base de Datos

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
- **6 Empresas** del complejo industrial
- **504 Empleados** con credenciales RFID únicas
- Servicios y registros históricos de días hábiles (enero, marzo y abril 2026)

---

## Cómo correr el proyecto

### Prerequisitos

- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9)
- SQL Server 2019+ (Express Edition es suficiente)
- Git

### Pasos

```bash
# 1. Clonar el repositorio
git clone https://github.com/f-Rra/Control-Almuerzos-MVC.git
cd Control-Almuerzos-MVC/SCA-MVC
```

```json
// 2. Configurar la connection string en appsettings.json
"ConnectionStrings": {
  "DefaultConnection": "Server=.\\SQLEXPRESS;Database=SCA_MVC;Trusted_Connection=True;TrustServerCertificate=True"
}
```

```bash
# 3. Aplicar migraciones y seed data
dotnet ef database update

# 4. Correr la aplicación
dotnet run
```

### Usuarios de demo

| Rol | Usuario | Contraseña |
|---|---|---|
| Admin | admin | Admin123! |
| Usuario | usuario | User123! |

> La contraseña del admin se configura en `appsettings.json` bajo `SeedAdmin:Password`.

---

## Herramientas y Tecnologías

**IDE y Entorno de Desarrollo:**
- **Visual Studio 2022 Community Edition**
- **SQL Server Management Studio (SSMS) 19**

**Frameworks y Librerías:**

| Paquete | Versión | Uso |
|---|---|---|
| **.NET 9** | 9.0 | Framework principal |
| **Entity Framework Core** | 9.0.0 | ORM — LINQ, migraciones, Fluent API |
| **ASP.NET Core Identity** | 9.0.0 | Autenticación, autorización y roles |
| **Bootstrap** | 5.3 | Framework CSS (distribución local) |
| **Bootstrap Icons** | 1.11.2 | Iconografía del sistema |
| **QuestPDF** | 2026.2.1 | Generación de PDFs |
| **MailKit / MimeKit** | 4.16.0 | Envío de emails vía SMTP |
| **Google Fonts (Outfit)** | — | Tipografía principal |

---

## Características Técnicas

### Seguridad

- **ASP.NET Core Identity**: Autenticación con hash de contraseñas (PBKDF2)
- **Roles**: `Admin` (acceso total) y `Usuario` (solo operativo)
- **[Authorize]**: En todos los controladores operacionales
- **[ValidateAntiForgeryToken]**: En todos los POST
- **Middleware global**: Captura de excepciones no controladas (404, 500)
- **Baja lógica**: No se eliminan datos, solo se desactivan
- **Integridad referencial**: Foreign Keys con `OnDelete(Restrict)`
- **Datos sensibles**: Contraseñas fuera del código fuente, en `appsettings.json`

### Rendimiento

- **AsNoTracking**: En todas las queries de solo lectura (6 servicios de negocio)
- **Agregaciones en BD**: `CountAsync`, `GroupBy` ejecutados en SQL Server, no en memoria
- **Inyección de dependencias**: Servicios `Scoped` por request
- **Vistas parciales**: Componentes reutilizables (`_ServicioCard`, `_KpiCard`, etc.)
- **AJAX**: Registro de comensales y filtros sin recargas de página

### Validaciones

**Base de Datos:** Unicidad de credenciales (Unique Index), integridad referencial (FK), check constraints, valores por defecto.

**Capa de negocio:** Empleado activo, sin duplicados en el mismo servicio, servicio activo requerido para registrar, credencial única al asignar, proyección en rango válido.

**Presentación:** jQuery Validation client-side, mensajería centralizada (`MensajesConstantes`), toasts de TempData (`MensajesUI`), errores de Identity en español (`SpanishIdentityErrorDescriber`).

---

## Guías de Desarrollo

La carpeta `/Guias/` contiene **51 guías** que documentan el proceso de desarrollo commit a commit. Cada guía detalla los archivos modificados, los conceptos técnicos implementados y las decisiones tomadas en ese punto del proyecto.

Las guías c01–c34 fueron redactadas **antes de cada commit**, como guías de implementación. Las guías c35–c51 fueron redactadas **después**, como documentación retrospectiva que incluye las herramientas de IA utilizadas en cada etapa.

---

## Metodología: Agentic Coding

El proyecto fue desarrollado bajo un modelo de **Ingeniería de Software Asistida por Agentes**: la arquitectura, las decisiones técnicas y la revisión de cada cambio estuvieron a cargo del autor; la implementación fue asistida por herramientas de IA con instrucciones precisas.

### Herramientas utilizadas

- **Claude Code (Anthropic):** Refactorización, migración ADO.NET → EF Core, arquitectura MVC, optimización de queries, auditoría de seguridad. Se configuraron skills personalizados (`/security-scan`, `/ef-core`, `/build-fix`, `/migrate`) y un `CLAUDE.md` con el contexto permanente del proyecto.
- **GitHub Copilot:** Autocompletado de sintaxis C#, consultas LINQ.

### Flujo de trabajo

Cada tarea siguió el mismo ciclo: el autor define el problema y el contexto → la IA propone la implementación → el autor revisa y aprueba o ajusta → se itera hasta el resultado correcto. Las guías de `/Guias/` documentan este proceso en detalle.

---

## Enlaces

- [Repositorio MVC](https://github.com/f-Rra/Control-Almuerzos-MVC)
- [Proyecto Original WinForms](https://github.com/f-Rra/Sistema-Control-Almuerzos)
- [Documentación .NET 9](https://learn.microsoft.com/en-us/dotnet/core/whats-new/dotnet-9)
- [Entity Framework Core](https://learn.microsoft.com/en-us/ef/core/)
- [QuestPDF](https://www.questpdf.com/)

---

**Facundo Herrera**
- Estudiante de Tecnicatura Universitaria en Programación — UTN FRGP
- Email: Facundo.herrera@alumnos.frgp.utn.edu.ar
