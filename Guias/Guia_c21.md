# Guía del Commit 21: Revisión del Modelado de Clases y Fluent API

## 🎯 Propósito de este Commit

El objetivo principal de este commit no es agregar nueva funcionalidad de negocio, sino **asegurar y documentar que el puente entre nuestra base de datos relacional y nuestro modelo de objetos (Entity Framework Core) sea exacto y fácil de entender**.

Entity Framework Core utiliza una técnica llamada **Fluent API** (dentro del método `OnModelCreating` en `ApplicationDbContext`) para configurar explícitamente cómo las clases (modelos) se mapean a las tablas de la base de datos. En este commit revisamos y explicamos cada una de estas configuraciones, lo que servirá como **referencia didáctica** para entender cómo EF Core diseña la base de datos bajo el capó.

## 📝 Conceptos de Fluent API Explicados

Dentro del archivo `ApplicationDbContext.cs`, hemos dejado comentarios explicativos en cada bloque. Aquí tienes el resumen de las herramientas principales utilizadas:

### 1. Relaciones Uno a Muchos (1:N)
Se configuran mediante la cadena `.HasMany().WithOne().HasForeignKey()`.
*   **Ejemplo:** `Empresa` tiene muchos `Empleados`.
*   Le decimos a EF Core: *"La entidad Empresa tiene una colección de Empleados (`HasMany`), y cada Empleado pertenece a una sola Empresa (`WithOne`), unidos por la clave foránea `IdEmpresa` (`HasForeignKey`)"*.

### 2. Comportamiento de Eliminación (`OnDelete`)
Controla qué pasa con los registros hijos cuando se elimina el registro padre.
*   `DeleteBehavior.Restrict`: Bloquea la eliminación. (Ej: No puedes borrar una Empresa si tiene Empleados activos).
*   `DeleteBehavior.SetNull`: Si borras el padre, la llave extranjera del hijo se pone en NULL. (Ej: Si un Empleado es eliminado físicamente, conservamos su Registro de asistencia, pero el `IdEmpleado` queda en nulo, representando a un "Invitado").

### 3. Índices Únicos y Filtros (`HasIndex().IsUnique()`)
Garantizan la integridad de los datos a nivel de base de datos.
*   **Ejemplo simple:** El `IdCredencial` de un Empleado debe ser único para que no existan tarjetas duplicadas.
*   **Ejemplo compuesto y filtrado:** Un empleado no puede registrarse dos veces en el mismo servicio. Usamos un índice compuesto de `IdEmpleado` + `IdServicio`. Además, le agregamos `.HasFilter("[IdEmpleado] IS NOT NULL")` para que esta regla **solo** aplique a empleados reales y no a los invitados (que tienen `IdEmpleado` nulo).

### 4. Valores por Defecto (`HasDefaultValue()`)
Asegura que ciertas columnas siempre tengan un valor inicial si no se especifica.
*   Por ejemplo, el campo `Estado` de la mayoría de las entidades comienza como `true` (Activo).
*   Los totales de comensales e invitados en un `Servicio` arrancan en `0`.

### 5. Restricciones de Validación (`HasCheckConstraint()`)
Agrega reglas de validación físicas directamente en SQL Server.
*   Agregamos un check que verifica que la `Fecha` de un servicio tenga lógica (no mayor al año 2030, útil para evitar errores de tipeo absurdos).

## 🛠️ Pasos a Seguir
1.  **Revisión de `ApplicationDbContext.cs`**: Se verificó que el interior del método `OnModelCreating` cumpla estrictamente con el esquema original de SQL de la Unidad 2.
2.  **Documentación en línea**: Se añadieron comentarios paso a paso (ya estaban pre-insertados, pero el commit actúa para fijar este hito de revisión y didáctica) detallando el "por qué" de cada línea de configuración.
3.  **Alineación**: Se confirmaron los tipos de datos en la base y los requerimientos opcionales (por ejemplo, el `int? IdEmpleado` para soportar invitados).

## 🚀 Cómo hacer tu Commit

Para realizar el paso 21 de manera oficial, abre tu terminal y ejecuta:

```bash
git add SCA-MVC/Data/ApplicationDbContext.cs
git commit -m "docs: documentar Fluent API en ApplicationDbContext" -m "- Comentarios explicativos en cada configuración" -m "- Detalle de relaciones, índices y restricciones" -m "- Referencia didáctica del modelado de clases con EF Core"
```

---

# Guía del Commit 22: Generación de Migración por Ajustes de Modelado

## 🎯 Propósito de este Commit

A lo largo del desarrollo inicial, y especialmente al documentar y verificar detalladamente las configuraciones de EF Core Fluent API, es posible que difiramos ligeramente (aun sin cambiar líneas de código, pueden ser tipos nativos) o simplemente hayamos generado un desfase del "Snapshot" interno de Entity Framework.

El objetivo de este commit es **hacer oficial cualquier ajuste de modelado** creando una nueva "Migración". Esto asegura que la clase que modela nuestra base de datos (`ApplicationDbContext`) esté en perfecta sincronía con el motor de base de datos SQL Server. 

## 📝 Conceptos Utilizados: Migraciones de EF Core

En lugar de crear tablas escribiendo código SQL (`CREATE TABLE ...`), Entity Framework usa **Migraciones**.
Cuando modificamos un bloque en `OnModelCreating` o añadimos propiedades a un modelo (como `.HasDefaultValue(true)`), necesitamos que EF Core traslade esos cambios al SQL. 

El flujo de trabajo es el siguiente:
1. `dotnet ef migrations add <NombreMigracion>`: Analiza el `DbContext`, lo compara con el estado anterior y genera un archivo `.cs` con las instrucciones C# de qué comandos SQL disparar (ej: crear tabla, alterar columna).
2. `dotnet ef database update`: Toma los archivos de migración no aplicados y los inyecta en SQL Server ejecutando el código SQL correspondiente en la base instalada.

## 🛠️ Pasos a Seguir
1.  **Ejecución de Comando de Migración**: Ejecutamos el comando de consola `dotnet ef migrations add AjustesModelo` que escaneó nuestras tablas y nuestro Fluent API.
2.  **Verificación**: EF Core creó una nueva migración con los pequeños ajustes y metadatos internos que puedan estar pendientes.
3.  **Actualización**: Se aplicó la migración mediante `dotnet ef database update`.

**Mensaje principal (Asunto):**
```text
docs, feat: documentar Fluent API de EF Core y aplicar migración de ajustes
```

**Descripción (Cuerpo):**
```text
- Comentarios explicativos en cada configuración
- Detalle de relaciones, índices y restricciones
- Referencia didáctica del modelado de clases con EF Core
- Revisión de tipos de datos contra esquema original
- Migración con correcciones de modelado
- Verificación de coherencia BD <-> Modelos
```
