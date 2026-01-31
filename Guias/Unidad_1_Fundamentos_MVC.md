# Unidad 1: Fundamentos de ASP.NET MVC

## Introducción al Patrón MVC

### ¿Qué es MVC?

**MVC (Model-View-Controller)** es un patrón de arquitectura de software que separa la aplicación en tres componentes principales:

- **Model (Modelo)**: Representa los datos y la lógica de negocio
- **View (Vista)**: Maneja la presentación y la interfaz de usuario
- **Controller (Controlador)**: Gestiona las peticiones del usuario y coordina el modelo y la vista

### Ventajas del Patrón MVC

1. **Separación de responsabilidades**: Cada componente tiene un propósito específico
2. **Facilita el testing**: Puedes probar cada capa de forma independiente
3. **Reutilización de código**: Los modelos y controladores pueden ser reutilizados
4. **Desarrollo paralelo**: Diferentes equipos pueden trabajar en diferentes capas
5. **Mantenibilidad**: El código es más fácil de mantener y escalar

---

## Comparación: Windows Forms vs ASP.NET MVC

### Windows Forms (Tu Aplicación Actual)

**Arquitectura:**
- Carpeta `dominio/` contiene los modelos de datos
- Carpeta `negocio/` contiene la lógica de negocio
- Carpeta `app/` contiene la interfaz de usuario (WinForms)
- Carpeta `app/UserControls/` contiene componentes visuales

**Características:**
- Aplicación de escritorio
- Interfaz gráfica con controles Windows
- Eventos de usuario (Click, KeyPress, etc.)
- Acceso directo a la base de datos desde la capa de negocio
- Ejecuta en una sola máquina

### ASP.NET MVC (Aplicación Web)

**Arquitectura:**
- Carpeta `Models/` equivale a `dominio/`
- Carpeta `Controllers/` equivale a la lógica de `negocio/`
- Carpeta `Views/` equivale a `UserControls/`
- Carpeta `Data/` maneja el acceso a datos con Entity Framework

**Características:**
- Aplicación web
- Interfaz HTML/CSS/JavaScript
- Peticiones HTTP (GET, POST)
- Acceso a datos mediante Entity Framework
- Ejecuta en un servidor, accesible desde navegadores

---

## Estructura de un Proyecto ASP.NET MVC

### Carpetas Principales

#### 1. Models/
Contiene las clases que representan los datos de tu aplicación.

**Migración desde WinForms:**
- `dominio/Empleado.cs` → `Models/Empleado.cs`
- `dominio/Empresa.cs` → `Models/Empresa.cs`
- `dominio/Servicio.cs` → `Models/Servicio.cs`

**Características:**
- Clases POCO (Plain Old CLR Objects)
- Propiedades con anotaciones de validación
- Relaciones entre entidades

#### 2. Controllers/
Contiene los controladores que manejan las peticiones HTTP.

**Migración desde WinForms:**
- `app/UserControls/ucEmpleados.cs` (eventos y lógica) → `Controllers/EmpleadosController.cs`
- `app/UserControls/ucEmpresas.cs` → `Controllers/EmpresasController.cs`

**Características:**
- Métodos de acción (ActionResult)
- Manejo de peticiones GET y POST
- Retorna vistas o datos JSON

#### 3. Views/
Contiene las vistas Razor (.cshtml) que generan HTML.

**Migración desde WinForms:**
- `app/UserControls/ucEmpleados.Designer.cs` (controles visuales) → `Views/Empleados/Index.cshtml`

**Características:**
- Sintaxis Razor (C# + HTML)
- Fuertemente tipadas (con modelos)
- Layouts compartidos

#### 4. wwwroot/
Contiene archivos estáticos (CSS, JavaScript, imágenes).

**Estructura típica:**
- `wwwroot/css/` - Hojas de estilo
- `wwwroot/js/` - Scripts JavaScript
- `wwwroot/images/` - Imágenes y recursos gráficos

---

## Ciclo de Vida de una Petición MVC

### Flujo Completo

1. **Usuario hace una petición** navegando a una URL
2. **Routing** determina qué controlador y acción ejecutar
3. **El controlador** procesa la petición
4. **El controlador** consulta/modifica el modelo (base de datos)
5. **El controlador** selecciona una vista
6. **La vista** genera HTML
7. **HTML** se envía al navegador del usuario

### Ejemplo Práctico: Listar Empleados

**Flujo:**
1. Usuario navega a `http://localhost/Empleados/Index`
2. Routing identifica el controlador `EmpleadosController` y la acción `Index()`
3. El controlador obtiene la lista de empleados desde la base de datos
4. El controlador pasa la lista a la vista `Index.cshtml`
5. La vista genera una tabla HTML con los empleados
6. El usuario ve la tabla en su navegador

---

## Routing en ASP.NET MVC

### ¿Qué es el Routing?

El **routing** es el mecanismo que mapea URLs a controladores y acciones.

### Patrón de URL por Defecto

**Formato:** `/{controller}/{action}/{id?}`

**Ejemplos:**
- `/Empleados/Index` → `EmpleadosController.Index()`
- `/Empleados/Detalles/5` → `EmpleadosController.Detalles(5)`
- `/Servicios/Crear` → `ServiciosController.Crear()`

### Configuración de Rutas

Las rutas se configuran en el archivo `Program.cs` del proyecto. La ruta por defecto apunta al controlador `Home` y la acción `Index`.

---

## Razor: Motor de Vistas

### ¿Qué es Razor?

**Razor** es un motor de plantillas que permite mezclar código C# con HTML de forma natural y legible.

### Sintaxis Básica

**Expresiones:**
- Mostrar propiedades del modelo
- Formatear fechas y números
- Ejecutar métodos

**Bloques de código:**
- Declarar variables
- Realizar cálculos
- Lógica condicional

**Condicionales:**
- Mostrar/ocultar elementos según condiciones
- Aplicar estilos diferentes según estado

**Bucles:**
- Iterar sobre listas
- Generar filas de tablas
- Crear elementos repetitivos

---

## Helpers HTML

### ¿Qué son los Helpers?

Los **HTML Helpers** son métodos que generan HTML de forma programática, facilitando la creación de formularios, enlaces y otros elementos.

### Helpers Comunes

**Enlaces:**
- Generar links a otras acciones
- Pasar parámetros en la URL
- Crear menús de navegación

**Formularios:**
- Crear formularios con método POST
- Generar campos de entrada
- Mostrar mensajes de validación
- Botones de envío

**Campos de entrada:**
- TextBox para texto
- DropDownList para selección
- CheckBox para valores booleanos
- Hidden para campos ocultos

---

## Layouts y Vistas Parciales

### Layouts

Un **layout** es una plantilla maestra que define la estructura común de todas las páginas (header, footer, menú de navegación).

**Características:**
- Define la estructura HTML base
- Incluye referencias a CSS y JavaScript
- Contiene el menú de navegación
- Define el área donde se inserta el contenido de cada vista
- Puede tener secciones opcionales para scripts

**Ubicación típica:** `Views/Shared/_Layout.cshtml`

### Vistas Parciales

Las **vistas parciales** son componentes reutilizables de UI que se pueden incluir en múltiples vistas.

**Casos de uso:**
- Tarjetas de información repetitivas
- Formularios que se usan en varias vistas
- Componentes de navegación
- Widgets de estadísticas

**Convención de nombres:** Comienzan con guion bajo (ej: `_EmpleadoCard.cshtml`)

---

## Mapeo de Conceptos: WinForms → MVC

### Interfaz de Usuario

| Windows Forms | ASP.NET MVC |
|---------------|-------------|
| `Form` | `View` (.cshtml) |
| `UserControl` | `Partial View` |
| `Label`, `TextBox`, `Button` | HTML + CSS + Razor |
| `DataGridView` | Tabla HTML con bucle |
| `MessageBox.Show()` | TempData + alertas en vista |

### Navegación

| Windows Forms | ASP.NET MVC |
|---------------|-------------|
| `Form.Show()` | `RedirectToAction()` |
| `Panel.Controls.Add()` | Helper de enlaces |
| Menú lateral con botones | Navbar con enlaces HTML |

### Eventos

| Windows Forms | ASP.NET MVC |
|---------------|-------------|
| `Button_Click` | Acción POST del controlador |
| `TextBox_KeyPress` | Formulario HTML + validación |
| `DataGridView_SelectionChanged` | Link a acción de detalles |
| `Timer_Tick` | JavaScript + AJAX |

### Datos

| Windows Forms | ASP.NET MVC |
|---------------|-------------|
| `AccesoDatos.cs` | Entity Framework |
| `SqlCommand` + `SqlDataReader` | LINQ to Entities |
| Mappers manuales | Automático con EF |
| Stored Procedures | LINQ o SP con EF |

---

## Preparación para la Migración

### Análisis de tu Aplicación Actual

**Módulos principales a migrar:**

1. **Gestión de Servicios**
   - Iniciar servicio
   - Finalizar servicio
   - Ver servicio activo

2. **Registro de Comensales**
   - Registro por credencial
   - Registro manual
   - Listado en tiempo real

3. **Gestión de Empleados**
   - CRUD completo
   - Asignación de credenciales

4. **Gestión de Empresas**
   - CRUD completo
   - Estadísticas por empresa

5. **Reportes**
   - Lista de servicios
   - Asistencias por empresa
   - Cobertura vs proyección
   - Distribución por día de semana

6. **Estadísticas**
   - Dashboard con métricas
   - Análisis temporal

7. **Configuración**
   - Información de BD
   - Sistema de respaldos

### Estrategia de Migración Recomendada

**Fase 1: Configuración Inicial**
- Crear proyecto ASP.NET Core MVC
- Configurar Entity Framework
- Migrar modelos de dominio

**Fase 2: Módulos Básicos**
- Gestión de Empresas (CRUD simple)
- Gestión de Empleados (CRUD con relaciones)

**Fase 3: Funcionalidad Principal**
- Gestión de Servicios
- Registro de Comensales

**Fase 4: Reportes y Análisis**
- Sistema de reportes
- Dashboard de estadísticas

**Fase 5: Configuración Avanzada**
- Respaldos de BD
- Configuración del sistema

---

## Creación del Proyecto

### Pasos para Crear el Proyecto

1. **Abrir Visual Studio 2022**
2. **Crear nuevo proyecto** → ASP.NET Core Web App (Model-View-Controller)
3. **Configurar el proyecto:**
   - Nombre: `SistemaControlAlmuerzos.Web`
   - Ubicación: Carpeta de tu preferencia
   - Framework: .NET 6.0 o superior
4. **Opciones adicionales:**
   - Tipo de autenticación: Ninguna (se configurará después)
   - Habilitar HTTPS: Sí
   - No usar top-level statements (opcional)

### Estructura Inicial del Proyecto

Al crear el proyecto, Visual Studio generará:
- Carpeta `Controllers/` con `HomeController.cs`
- Carpeta `Models/` vacía
- Carpeta `Views/` con vistas de ejemplo
- Carpeta `wwwroot/` con archivos estáticos
- Archivo `Program.cs` con configuración
- Archivo `appsettings.json` para configuración

---

## Configuración Inicial

### Cadena de Conexión

Deberás configurar la cadena de conexión a tu base de datos SQL Server en el archivo `appsettings.json`.

**Elementos a configurar:**
- Servidor de base de datos
- Nombre de la base de datos
- Autenticación (Windows o SQL Server)
- Opciones de seguridad

### Instalación de Paquetes NuGet

Necesitarás instalar los siguientes paquetes:
- **Microsoft.EntityFrameworkCore.SqlServer** - Para conectar con SQL Server
- **Microsoft.EntityFrameworkCore.Tools** - Para migraciones
- **Microsoft.EntityFrameworkCore.Design** - Para diseño de base de datos

### Configuración de Entity Framework

Deberás crear una clase `ApplicationDbContext` que heredará de `DbContext` y contendrá los DbSets para cada entidad de tu sistema.

---

