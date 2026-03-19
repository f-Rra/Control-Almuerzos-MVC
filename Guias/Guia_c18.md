# Guía Commit 18 — Seed de Datos Iniciales (Data Seeding de Demostración)

## 🎯 Propósito

Precargar el entorno Base de Datos mediante el "Método de Configuración de Modelos" (`OnModelCreating` de Entity Framework Core) para que sirva de ambiente de desarrollo (Sandboxing). Esto carga de manera fija empresas, lugares y empleados inventados para que los reportes, visuales y KPIs del Commit 16 y 17 tengan datos con los que mostrar sus capacidades al instante al compilar de cero.

## 📝 Concepto Central

Anteriormente en Winforms teníamos un script `Datos_Iniciales.sql`. En MVC implementaremos el estándar de Entity Framework: **HasData**. En lugar de ejecutar queries manuales, proveemos instancias de C# con IDs duros (hardcoded) en el `ModelBuilder`. Al generar una migración y actualizar la base, EF Core realiza la inserción automáticamente sin romper nuestra estructura.

## ⚡ Pasos a Seguir

1. **Context Db (`Data/ApplicationDbContext.cs`)**:
   - En el método `OnModelCreating`, utilizar bloques fluidos: `modelBuilder.Entity<Lugar>().HasData(...)`.
   - Instanciar a mano y explícitamente el `Id` de las 6 Empresas clave (e.g. Roemmers, Gema).
   - Instanciar 16 Empleados inventados usando credenciales coherentes (`RF001` - `RF016`). Asignarles un `IdEmpresa` estático proveniente del bloque anterior.
2. **Soporte Falso para Registros Históricos**:
   - Sembrar algunos "Servicios" y "Registros" de febrero 2026 para que el Dashboard ya comience mostrando actividad.
   - Si la DB o ModelBuilder tiene una Constraint (`CK_Servicio_Fecha`) muy restrictiva al presente, relajarla en la definición SQL para soportar nuestra época del testeo ("febrero 2026").
3. **Aplicar Migraciones**:
   - Consolidar en Consola (Package Manager / Bash): `dotnet ef migrations add DemostracionSeeding` seguida de `dotnet ef database update`.

## 📁 Archivos a Crear y Modificar

| Archivo | Acción | Descripción |
|---|---|---|
| `Data/ApplicationDbContext.cs` | Modificar | Adicionar objetos nativos en bloque `HasData` |
| `Migrations/*Seed*` | Crear | Documento de iteración de EF para autocompletado en nuevas DB |

## 🚀 Mensaje de Commit

```
feat: implementar seed de datos iniciales con ModelBuilder

- Carga inicial de Lugares (Comedor, Quincho)
- 6 empresas del complejo (Roemmers, Gema, etc.)
- 16 empleados con credenciales RFxxxx y nombres realistas
- Muestra de servicios y registros para febrero 2026
- Ajuste de CK_Servicio_Fecha para permitir datos de demo
```
