# Guía Commit 11 — Servicios de Negocio (Negocio Layer)

## 🎯 Propósito

Crear la capa de servicios de negocio estableciendo interfaces claras para cada modelo que actúen como puente (`Repository Pattern` simple) entre los Controladores (UI) y la Base de Datos (a través del `AccesoDatos` recién creado).

## 📝 Concepto Central

Los Controladores no deben interactuar nunca directamente con `AccesoDatos` ni con `SqlConnection`. Deben solicitar datos a una **Interfaz de Negocio** (por ejemplo, `IEmpresaNegocio`). De esta forma, cualquier cambio futuro en el motor de persistencia (como una eventual migración a Entity Framework Core) resultará invisible para las Vistas y Controladores. Este es el contrato que garantiza un bajo acoplamiento.

## ⚡ Pasos a Seguir

1. **Definir Interfaces (`Services/I...Negocio.cs`)**:
   - Crear los contratos para `IEmpresaNegocio`, `IEmpleadoNegocio`, `IServicioNegocio`, `IRegistroNegocio` e `ILugarNegocio`.
   - Especificar sus correspondientes métodos CRUD según el documento original (ej. `Listar()`, `Agregar()`).
2. **Implementar las Clases de Lógica (`Services/...Negocio.cs`)**:
   - En cada clase que implemente su interfaz, inyectar por constructor `AccesoDatos`.
   - Encapsular cada llamada con instanciaciones como: `datos.SetearProcedimiento("sp_...");`, `datos.EjecutarLectura();`, extraer usando el Mapper, y cerrar.
   - Todo bloque debe capturar Excepciones propias devolviendo vacíos o falseando retornos lógicos.
3. **Registro en Inyección de Dependencias**:
   - En `Program.cs`, registrar todas las capas `builder.Services.AddScoped<IEntidadNegocio, EntidadNegocio>();`.

## 📁 Archivos a Crear y Modificar

| Archivo | Acción | Descripción |
|---|---|---|
| `Services/I*Negocio.cs` | Crear | Contratos inyectables (5 archivos) |
| `Services/*Negocio.cs` | Crear | Clases de lógica puente hacia SPs (5 archivos) |
| `Program.cs` | Modificar | Registrar dependencias Scoped al contenedor MVC |

## 🚀 Mensaje de Commit

```
feat: crear capa de servicios de negocio con ADO.NET

- Servicios para Empresa, Empleado, Servicio, Registro y Lugar
- Interfaces para inyección de dependencias
- Llamadas a stored procedures via AccesoDatos
- Registro de servicios en el contenedor de DI
```
