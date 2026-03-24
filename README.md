
Migración completa del [Sistema Control de Almuerzos](https://github.com/f-Rra/Sistema-Control-Almuerzos) a **ASP.NET Core MVC**, manteniendo todas las funcionalidades del sistema original y agregando nuevas capacidades propias de una aplicación web: autenticación con Identity, roles y permisos, envío de reportes por email, diseño responsivo y gestión de usuarios.

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

### Dashboard 

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

### Panel de Administración

Punto de acceso centralizado a todas las funciones administrativas:

- **Empresas**: Gestión completa de empresas del predio
- **Empleados**: Administración de empleados y credenciales
- **Estadísticas**: Dashboard de análisis y métricas del sistema
- **Usuarios**: Gestión de cuentas, roles y accesos

---

### Gestión de Empresas

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

### Gestión de Empleados

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

### Gestión de Usuarios

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
│   │   ├── css/site.css              # Estilos globales 
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

## Metodología de Desarrollo Asistido por IA 

Este proyecto fue desarrollado bajo un paradigma moderno de **Ingeniería de Software Asistida por Agentes**. Toda la arquitectura base, la lógica de negocio original y la toma de decisiones críticas fueron dirigidas y supervisadas por mí (el autor), mientras que la escritura de código, refactorización y documentación fue delegada a distintas inteligencias artificiales.

### Ecosistema de Herramientas Utilizadas

- **Claude Code:** Utilizado para la refactorización profunda, migración de capas (ADO.NET a Entity Framework) y estructuración de la lógica MVC.
- **Antigravity (Google DeepMind):** Empleado como agente autónomo para la creación interactiva de componentes UI, resolución de bugs de responsividad y generación de documentación dinámica.
- **GitHub Copilot:** Asistencia en tiempo real para el autocompletado de sintaxis C#, consultas LINQ y validaciones de seguridad.

### Control de Calidad y Código Limpio

El uso de herramientas generativas no solo redujo drásticamente los tiempos de desarrollo, sino que también permitió mantener estándares de código limpio mucho más altos de lo habitual. Mediante ciclos de revisión constante, piezas complejas fueron fragmentadas en componentes MVC puros y reutilizables de forma segura.

### Evolución y Velocidad del Proyecto

A lo largo de la construcción de las vistas estáticas iniciales hasta la refactorización profunda de ADO.NET a Entity Framework Core, la sinergia entre el criterio técnico humano y la velocidad de iteración algorítmica demostró ser la principal ventaja de este modelo de desarrollo, logrando un código final altamente cohesionado y con baja tasa de defectos.

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