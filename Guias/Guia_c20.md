# Guía Paso a Paso: Commit 20 - Refactorización de ADO.NET a Entity Framework Core

## 🎯 Propósito del Commit

El objetivo principal de este commit es **modernizar y simplificar la capa de acceso a datos** del proyecto. Hasta ahora, la aplicación utilizaba ADO.NET clásico (clase `AccesoDatos.cs`) interactuando directamente con Procedimientos Almacenados (Stored Procedures) y requiriendo "Mappers" manuales para convertir las filas de la base de datos en objetos C#.

Al migrar a **Entity Framework Core (EF Core)**, logramos:

1. **Eliminar código repetitivo (Boilerplate):** EF Core se encarga automáticamente de abrir conexiones, ejecutar consultas estructurales y mapear resultados a objetos (modelos).
2. **Consultas fuertemente tipadas:** En lugar de invocar SPs con nombres en strings mágicos (`"sp_ListarEmpleados"`), pasaremos a utilizar LINQ puro (`_context.Empleados.ToList()`), lo cual provee chequeo de errores en tiempo de compilación.
3. **Mantenimiento Simplificado:** Al depender de `ApplicationDbContext` y `DbSet<T>`, cualquier cambio futuro en la estructura de la base de datos se refleja y resuelve directamente a nivel de código.
4. **Respetar la Arquitectura:** Los Controladores de tu aplicación web **NO DEBEN CAMBIAR EN ABSOLUTO**. Dado que hemos utilizado inyección de dependencias a través de interfaces (ej. `IEmpresaNegocio`), los controladores seguirán pidiendo datos a la interfaz; sólo cambiaremos el motor interno bajo el capó de cada servicio.

Esta refactorización marca el punto de convergencia donde el proyecto adopta los mismos estándares que enseñados en el "Curso Nivel 4: MVC + Entity Framework", dejándolo verdaderamente alineado con las tecnologías modernas de .NET.

---

## 🗑️ Paso 1: Limpieza Histórica (Eliminación de ADO.NET)

El primer paso para abrazar EF Core es soltar el pasado. Debemos eliminar todos los archivos y estructuras que se encargaban del puente manual con la base de datos.

**Archivos y Carpetas a Eliminar:**
- `Data/AccesoDatos.cs`: Esta es nuestra envoltura vieja de `SqlConnection` y `SqlCommand`. Ya no es necesaria.
- Carpeta `Data/Mappers/` (y todo su contenido): EF Core hace el mapeo de columnas a propiedades automáticamente, por lo que estas clases quedan obsoletas.
- `Data/NegocioException.cs`: Podríamos conservar una excepción personalizada si lo deseamos, pero usualmente dejaremos que EF Core lance sus propias excepciones (`DbUpdateException`). Puedes eliminarlo para simplificar.

---

## ⚙️ Paso 2: Inyección de Dependencias en Servicios

En todos nuestros servicios (capa *Negocio*), debemos cambiar cómo interactuamos con la base de datos.

1. **Modificar Constructores:** Donde antes instanciábamos `AccesoDatos` o lo inyectábamos, ahora pediremos por el constructor una instancia de `ApplicationDbContext`.
2. **Campos Privados:** Se debe crear un campo `private readonly ApplicationDbContext _context;` en cada servicio que lo requiera.

**Archivos a Modificar:**
- `Services/EmpresaNegocio.cs`
- `Services/EmpleadoNegocio.cs`
- `Services/LugarNegocio.cs`
- `Services/ServicioNegocio.cs`
- `Services/RegistroNegocio.cs`
- `Services/ReporteNegocio.cs`
- `Services/EstadisticasNegocio.cs`

---

## 🔄 Paso 3: Traducción de Stored Procedures a LINQ

Este es el proceso más importante. Para cada método listado en nuestras interfaces de negocio, debemos borrar la lógica de `try/catch` que utiliza `AccesoDatos` y reemplazarla por su equivalente en LINQ usando `_context`.

### 3.1. Consultas y Obtención de Datos (Read)
- **Selects Básicos:** Reemplazar llamadas a listar todo por operaciones LINQ como `.ToListAsync()`.
- **Búsqueda por ID:** Utilizar `.FindAsync(id)` o `.FirstOrDefaultAsync(e => e.Id == id)`.
- **Relaciones (Joins):** En ADO.NET usabas un SP complejo o hacías un Join en memoria. Con EF Core, debes utilizar explícitamente el modificador `.Include()` para traer datos relacionados (por ejemplo, buscar empleados e `.Include(e => e.Empresa)` para que traiga la información de la empresa).

### 3.2. Filtros y Búsquedas (Read / Filter)
- Reemplazar funciones de SP condicionales con sentencias `Where()`. Ejemplo: buscar por nombre implicará una evaluación LINQ `.Where(e => e.Nombre.Contains(texto))`.

### 3.3 Alta, Baja y Modificación (CUD)
- **Insertar (Create):** Usar el comando `_context.[Entidad].Add(modelo)` seguido exhaustivamente de `await _context.SaveChangesAsync()`.
- **Actualizar (Update):** Recuperar el objeto base, modificar sus propiedades, y llamar a `_context.SaveChangesAsync()`. EF Core "observa" los cambios y hace el `UPDATE`. También puedes usar `_context.Update(modelo)` si estás usando objetos desconectados de la UI.
- **Baja Física/Lógica (Delete):**
  - Físico: `_context.[Entidad].Remove(modelo)`.
  - Lógica: En lugar de borrar, buscas la entidad y le asignas `.Estado = false`, luego guardas los cambios.

---

## 📊 Paso 4: Consideraciones Especiales (Reportes y Estadísticas)

Los archivos `ReporteNegocio` y `EstadisticasNegocio` son los más complejos porque antes encapsulaban llamadas a **Procedimientos Almacenados complejos y Vistas**.

1. **Decisión Arquitectónica:** Tienes dos caminos con EF Core:
   - **Camino A (Purista):** Reescribir las consultas SQL y agrupamientos complejos de reportes utilizando la sintaxis de agrupación y proyecciones de LINQ (`GroupBy`, `Select()`, `.Count()`).
   - **Camino B (Pragmático):** Entity Framework Core tiene la capacidad de llamar a Vistas y Procedimientos Almacenados si mapeas entidades de respuesta directa (usando `FromSqlRaw` o configurando `Keyless Entity Types` en el Contexto).

Te sugiero que para las acciones CRUD y búsquedas estándar utilices el **Camino A** (LINQ Puro). Para las estadísticas matemáticas complejas y masivas (donde el SQL Engine es mucho más eficiente), evalúes si vale la pena mantener la llamada al SP a través del DbContext utilizando `_context.Database.ExecuteSqlRawAsync(...)` o `_context.Set<MiReporteViewModel>().FromSqlRaw(...)`.

---

## 🚀 Paso 5: Compilación y Pruebas Iniciales

Dado que todo este refactor cambia los cimientos de la aplicación sin alterar los controladores, tu mejor test será compilar el proyecto.

1. Al intentar hacer el "Build", Visual Studio te gritará sobre referencias a `AccesoDatos` o funciones de negocio obsoletas.
2. Una vez que la compilación sea exitosa y libre de errores, el comportamiento de las grillas de empleados, empresas y demás deberá ser idéntico al de antes para el usuario final, pero abismalmente distinto internamente.

---

## 📝 Mensaje de Commit Propuesto

Una vez finalizada la implementación y probada de inicio a fin:

**Asunto:**
`refactor: reemplazar ADO.NET manual por consultas estructuradas de Entity Framework Core`

**Cuerpo:**
`- Eliminar la clase antigua AccesoDatos.cs y la carpeta de Mappers en memoria`
`- Actualizar servicios (Negocio) inyectando ApplicationDbContext`
`- Traducir consultas SQL en crudo y SPs básicos a operaciones LINQ (.Where, .Include, .FirstOrDefault)`
`- Reemplazar operaciones manuales de inserción y modificación por .Add(), .Update() y SaveChangesAsync()`
`- Mantener la integridad del contrato de las interfaces para no impactar a los Controladores MVC`
