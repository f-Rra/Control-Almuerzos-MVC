# Guía Commit 9 — Capa de Acceso a Datos (ADO.NET) y Mappers

## 🎯 Propósito

Crear la capa fundacional de conexión a base de datos de la Unidad 2, mediante el patrón ADO.NET clásico, para preparar los cimientos que conectarán nuestros modelos C# con las tablas de SQL Server y los Procedimientos Almacenados.

## 📝 Concepto Central

Construiremos un encapsulador (Wrapper) llamado `AccesoDatos` que administre eficientemente los `SqlConnection` y `SqlCommand`. Como ADO.NET devuelve la información cruda en formato `SqlDataReader`, se necesita implementar un "Mapper" para cada entidad que traduzca las columnas de SQL a propiedades de C# estructuradas.

## ⚡ Pasos a Seguir

1. **Crear Motor Base (`Data/AccesoDatos.cs`)**:
   - Programar métodos `SetearProcedimiento()`, `SetearParametro()`, `EjecutarLectura()`, `EjecutarAccion()`, `CerrarConexion()`.
   - Incluir inyección de `IConfiguration` para leer la connection string desde `appsettings.json`.
2. **Excepción de Negocio (`Data/NegocioException.cs`)**:
   - Crear una excepción personalizada manejadora de errores de SQL.
3. **Mapeo Fijo (`Data/Mappers/`)**:
   - Crear `EmpleadoMapper`, `EmpresaMapper`, `ServicioMapper`, etc.
   - Construir en cada uno su método `Map(SqlDataReader reader)` realizando casteos explícitos (e.g. `(int)reader["IdEmpleado"]`).
4. **Registro DI**:
   - Añadir servicio `builder.Services.AddScoped<AccesoDatos>();` en `Program.cs`.

## 📁 Archivos a Crear y Modificar

| Archivo | Acción | Descripción |
|---|---|---|
| `Data/AccesoDatos.cs` | Crear | Wrapper ADO.NET |
| `Data/NegocioException.cs` | Crear | Capturador de errores de DB |
| `Data/Mappers/*.cs` | Crear | Traductores reader a objeto (Empleado, Empresa, etc.) |
| `Program.cs` | Modificar | Registro del wrapper en la Inyección de Dependencias |

## 🚀 Mensaje de Commit

```
feat: crear capa de acceso a datos con ADO.NET

- Clase AccesoDatos con SqlConnection/SqlCommand/SqlDataReader
- NegocioException con traducción de errores SQL a español
- Mappers estáticos para cada entidad del dominio
- Registro en contenedor de inyección de dependencias
```
