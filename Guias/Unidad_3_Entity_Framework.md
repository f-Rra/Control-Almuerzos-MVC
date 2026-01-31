# Unidad 3: Entity Framework Core

## Introducción a Entity Framework Core

**Entity Framework Core (EF Core)** es un ORM (Object-Relational Mapper) moderno que permite trabajar con bases de datos usando objetos .NET, eliminando la necesidad de escribir SQL manualmente en la mayoría de los casos.

### Ventajas de EF Core

1. **Productividad**: Menos código SQL manual
2. **Type-Safety**: Errores detectados en tiempo de compilación
3. **Migraciones**: Control de versiones de la base de datos
4. **LINQ**: Consultas fuertemente tipadas
5. **Tracking**: Seguimiento automático de cambios

---

## Comparación: AccesoDatos.cs vs Entity Framework

### Tu Sistema Actual (WinForms)

**Clase AccesoDatos.cs:**
- Maneja conexiones SQL manualmente
- Crea comandos SQL o llama stored procedures
- Ejecuta lecturas con SqlDataReader
- Ejecuta acciones con ExecuteNonQuery
- Requiere mapeo manual de datos a objetos

**Características:**
- Control total sobre las consultas SQL
- Gestión manual de conexiones
- Mappers personalizados para convertir DataReader a objetos
- Manejo explícito de parámetros

### Con Entity Framework Core

**Clase ApplicationDbContext:**
- Gestiona conexiones automáticamente
- Genera SQL automáticamente desde consultas LINQ
- Mapea automáticamente resultados a objetos
- Tracking automático de cambios
- Transacciones implícitas

**Características:**
- Consultas fuertemente tipadas con LINQ
- Mapeo automático objeto-relacional
- Migraciones para evolución del esquema
- Lazy loading, eager loading y explicit loading

---

## Configuración de Entity Framework

### 1. Instalación de Paquetes NuGet

Necesitarás instalar tres paquetes principales:
- **Microsoft.EntityFrameworkCore.SqlServer** - Proveedor para SQL Server
- **Microsoft.EntityFrameworkCore.Tools** - Herramientas para migraciones
- **Microsoft.EntityFrameworkCore.Design** - Soporte en tiempo de diseño

### 2. Creación del DbContext

**ApplicationDbContext.cs:**

Deberás crear una clase que herede de `DbContext` con las siguientes características:

**Constructor:**
- Recibe `DbContextOptions<ApplicationDbContext>` como parámetro
- Llama al constructor base con las opciones

**DbSets (Propiedades):**
- Un DbSet por cada entidad (Empleados, Empresas, Lugares, Servicios, Registros)
- Cada DbSet representa una tabla en la base de datos

**Método OnModelCreating:**
- Configura las relaciones entre entidades
- Define restricciones y validaciones
- Configura índices
- Establece valores por defecto
- Configura nombres de tablas

### 3. Configuración en Program.cs

**Registro del DbContext:**
- Se agrega el DbContext como servicio
- Se configura la cadena de conexión desde appsettings.json
- Se habilita la inyección de dependencias

### 4. Cadena de Conexión en appsettings.json

**Configuración necesaria:**
- Sección "ConnectionStrings"
- Nombre de la conexión (típicamente "DefaultConnection")
- Servidor, base de datos, autenticación
- Opciones de seguridad y confianza

---

## Configuración de Entidades

### Configuración de Empleado

**Aspectos a configurar:**
- Nombre de la tabla (EMPLEADOS)
- Clave primaria (IdEmpleado)
- Propiedades requeridas y longitudes máximas
- Valores por defecto (Estado = true)
- Índice único para IdCredencial
- Relación con Empresa (muchos a uno)
- Relación con Registros (uno a muchos)
- Comportamiento al eliminar (Restrict)

### Configuración de Empresa

**Aspectos a configurar:**
- Nombre de la tabla (EMPRESAS)
- Clave primaria (IdEmpresa)
- Propiedades requeridas
- Valores por defecto
- Relación con Empleados (uno a muchos)

### Configuración de Servicio

**Aspectos a configurar:**
- Nombre de la tabla (SERVICIOS)
- Clave primaria (IdServicio)
- Propiedades requeridas y valores por defecto
- Relación con Lugar
- Relación con Registros
- Comportamiento al eliminar

### Configuración de Registro

**Aspectos a configurar:**
- Nombre de la tabla (REGISTROS)
- Clave primaria (IdRegistro)
- Múltiples relaciones (Empleado, Empresa, Servicio, Lugar)
- Todas las relaciones con DeleteBehavior.Restrict

---

## Migraciones de Base de Datos

### ¿Qué son las Migraciones?

Las **migraciones** son una forma de mantener el esquema de la base de datos sincronizado con el modelo de datos de tu aplicación, manteniendo un historial de cambios.

### Comandos Básicos

**Crear migración inicial:**
- Comando: `dotnet ef migrations add MigracionInicial`
- Genera una carpeta `Migrations/` con el código de la migración
- Crea un snapshot del modelo actual

**Aplicar migración a la base de datos:**
- Comando: `dotnet ef database update`
- Ejecuta las migraciones pendientes
- Actualiza el esquema de la base de datos

**Ver migraciones pendientes:**
- Comando: `dotnet ef migrations list`
- Muestra todas las migraciones y su estado

**Revertir última migración:**
- Comando: `dotnet ef database update NombreMigracionAnterior`
- Revierte los cambios de la última migración

**Eliminar última migración (si no se aplicó):**
- Comando: `dotnet ef migrations remove`
- Elimina la última migración no aplicada

### Flujo de Trabajo con Migraciones

**Proceso típico:**
1. Modificar modelos (agregar propiedad, cambiar tipo, etc.)
2. Crear migración con nombre descriptivo
3. Revisar código generado en carpeta Migrations/
4. Aplicar a BD con database update
5. Verificar cambios en SQL Server Management Studio

### Ejemplo de Migración

**Escenario:** Agregar campo Email a Empleado

**Pasos:**
1. Agregar propiedad Email al modelo Empleado
2. Agregar Data Annotations (StringLength, EmailAddress)
3. Crear migración: `dotnet ef migrations add AgregarEmailEmpleado`
4. Revisar el código generado (método Up y Down)
5. Aplicar: `dotnet ef database update`
6. Verificar en SSMS que la columna se agregó

---

## Consultas LINQ

### Operaciones Básicas

#### Listar Todos

**Migración desde WinForms:**
- **Antes:** AccesoDatos con stored procedure, SqlDataReader, mapeo manual
- **Ahora:** Consulta LINQ simple con ToListAsync()

**Características:**
- Include() para cargar relaciones
- Async/await para operaciones asíncronas
- Mapeo automático a objetos

#### Buscar por ID

**Migración desde WinForms:**
- **Antes:** Stored procedure con parámetro, lectura manual
- **Ahora:** FirstOrDefaultAsync() con expresión lambda

**Características:**
- Retorna null si no encuentra
- Include() para relaciones
- Type-safe

#### Filtrar

**Migración desde WinForms:**
- **Antes:** Consulta SQL con WHERE y parámetros
- **Ahora:** Where() con expresiones lambda

**Características:**
- Múltiples condiciones con operadores lógicos
- Fuertemente tipado
- IntelliSense completo

### Consultas Avanzadas

#### Búsqueda con Múltiples Criterios

**Características:**
- Construir consulta base con AsQueryable()
- Agregar filtros condicionalmente
- Aplicar ordenamiento
- Ejecutar con ToListAsync()

**Casos de uso:**
- Búsqueda de empleados por nombre, empresa, estado
- Filtros opcionales que se aplican solo si tienen valor

#### Proyecciones (Select)

**Características:**
- Seleccionar solo las propiedades necesarias
- Crear objetos anónimos o ViewModels
- Reducir datos transferidos
- Mejorar rendimiento

**Casos de uso:**
- Listados simplificados
- Datos para DropDownLists
- Reportes con datos calculados

#### Agrupaciones

**Características:**
- GroupBy() para agrupar por criterio
- Select() para proyectar resultados
- Funciones de agregación (Count, Sum, Average)

**Casos de uso:**
- Contar empleados por empresa
- Sumar asistencias por período
- Promedios y estadísticas

#### Joins y Relaciones

**Características:**
- Include() para eager loading
- ThenInclude() para relaciones anidadas
- Múltiples Include() para varias relaciones

**Casos de uso:**
- Registros con empleado, empresa, servicio y lugar
- Servicios con lugar y todos sus registros
- Empleados con empresa y sus registros

---

## Uso de Stored Procedures con EF Core

### Ejecutar SP que Retorna Datos

**Método FromSqlRaw:**
- Permite ejecutar stored procedures
- Recibe parámetros SQL
- Retorna entidades mapeadas
- Se puede combinar con Include()

**Migración de tus SPs:**
- sp_ListarEmpleados
- sp_ListarServiciosRango
- sp_AsistenciasPorEmpresas

### Ejecutar SP de Acción

**Método ExecuteSqlRawAsync:**
- Para SPs que no retornan datos
- Manejo de parámetros de salida
- Retorna número de filas afectadas

**Migración de tus SPs:**
- sp_RegistrarAlmuerzo
- sp_IniciarServicio
- sp_FinalizarServicio

---

## Transacciones

### Transacciones Automáticas

**SaveChangesAsync():**
- Automáticamente envuelve cambios en una transacción
- Si falla, hace rollback automático
- Múltiples operaciones se ejecutan como unidad atómica

**Casos de uso:**
- Crear empleado y asignar credencial
- Registrar almuerzo y actualizar contador

### Transacciones Manuales

**BeginTransactionAsync():**
- Control explícito de transacciones
- Múltiples SaveChangesAsync() en una transacción
- Commit o Rollback manual

**Casos de uso:**
- Iniciar servicio y registrar invitados
- Operaciones complejas con múltiples pasos
- Necesidad de rollback condicional

---

## Optimización de Consultas

### Eager Loading (Carga Anticipada)

**Include() y ThenInclude():**
- Carga relaciones en la misma consulta
- Evita el problema N+1
- Una sola consulta SQL con JOINs

**Cuándo usar:**
- Cuando sabes que necesitarás las relaciones
- Listados que muestran datos relacionados
- Detalles completos de una entidad

### Lazy Loading (Carga Perezosa)

**Características:**
- Carga relaciones bajo demanda
- Requiere configuración adicional
- Puede causar problemas de rendimiento

**Cuándo usar:**
- No recomendado para aplicaciones web
- Solo si realmente se necesita

### Explicit Loading (Carga Explícita)

**Entry().Reference() y Entry().Collection():**
- Carga relaciones explícitamente cuando se necesitan
- Control fino sobre qué cargar
- Útil para escenarios específicos

**Cuándo usar:**
- Cargar relaciones condicionalmente
- Optimizar consultas específicas

### AsNoTracking para Consultas de Solo Lectura

**Características:**
- No rastrea cambios en las entidades
- Mejor rendimiento
- Menor uso de memoria

**Cuándo usar:**
- Listados que no se modificarán
- Consultas de solo lectura
- Reportes y estadísticas

---

## Migración de Procedimientos Almacenados

### Estrategia de Migración

**Opción 1: Reemplazar con LINQ**
- **Ventajas:** Más mantenible, type-safe, portable
- **Desventajas:** Puede ser menos eficiente en casos complejos
- **Cuándo usar:** Lógica simple, consultas estándar

**Opción 2: Mantener SPs**
- **Ventajas:** Reutilizar lógica existente, mejor rendimiento en algunos casos
- **Desventajas:** Menos portable, más difícil de mantener
- **Cuándo usar:** Lógica compleja, optimizaciones específicas de SQL Server

### Ejemplos de Migración

**sp_ListarEmpleados:**
- **Opción LINQ:** Consulta simple con Include()
- **Resultado:** Mismo resultado, más mantenible

**sp_RegistrarAlmuerzo:**
- **Opción LINQ:** Múltiples operaciones con SaveChangesAsync()
- **Opción SP:** Mantener el SP y llamarlo con ExecuteSqlRawAsync()

**sp_ReportePorPeriodo:**
- **Opción LINQ:** Consulta con Where(), GroupBy(), Select()
- **Opción SP:** Mantener si tiene lógica compleja

---

## Seed Data (Datos Iniciales)

### Configurar Datos Iniciales

**Método HasData() en OnModelCreating:**
- Permite definir datos iniciales
- Se incluyen en las migraciones
- Útil para datos de referencia

**Casos de uso:**
- Lugares (Comedor, Quincho)
- Empresas iniciales
- Datos de configuración

**Características:**
- Los datos se insertan al aplicar migraciones
- Se pueden actualizar en migraciones posteriores
- Requieren IDs explícitos

---

## Ejercicios Prácticos

### Ejercicio 1: Crear DbContext Completo

**Tareas:**
1. Crear la clase ApplicationDbContext
2. Agregar todos los DbSets
3. Configurar todas las entidades en OnModelCreating
4. Definir relaciones y restricciones
5. Configurar índices únicos

### Ejercicio 2: Migración Inicial

**Tareas:**
1. Crear la migración inicial
2. Revisar el código generado
3. Aplicar la migración a la base de datos
4. Verificar las tablas en SSMS
5. Insertar datos de prueba

### Ejercicio 3: Consultas LINQ

**Tareas:**
1. Listar empleados activos de una empresa específica
2. Obtener servicios del mes actual con sus registros
3. Calcular top 5 empresas con más asistencias
4. Buscar empleados por nombre o credencial
5. Obtener estadísticas de un servicio

---

## Resumen de la Unidad

### Lo que Aprendiste

✅ Qué es Entity Framework Core y sus ventajas
✅ Diferencias entre AccesoDatos.cs y EF Core
✅ Configuración del DbContext
✅ Migraciones de base de datos
✅ Consultas LINQ básicas y avanzadas
✅ Uso de Stored Procedures con EF Core
✅ Transacciones automáticas y manuales
✅ Optimización de consultas
✅ Estrategias de migración de SPs

### Archivos Creados

**Configuración:**
- `Data/ApplicationDbContext.cs`
- `appsettings.json` (cadena de conexión)
- `Program.cs` (registro del DbContext)

**Migraciones:**
- `Migrations/xxxxx_MigracionInicial.cs`
- `Migrations/ApplicationDbContextModelSnapshot.cs`

---
