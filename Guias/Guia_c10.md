# Guía Commit 10 — Stored Procedures, Vistas y Triggers en SQL Server

## 🎯 Propósito

Impactar físicamente la base de datos inyectando un bloque integral de **33 Procedimientos Almacenados (SPs)**, Vistas y Triggers que manejarán las operaciones SQL para los servicios ADO.NET, manteniendo viva la lógica estructurada de validación y extracción original.

## 📝 Concepto Central

El uso intensivo de Procedimientos Almacenados permite encapsular la complejidad (filtraje múltiple de reportes, lógica de JOINs entre empresas/empleados, y validaciones "anti-trampa" y unicidad física del servicio activo) en el motor de base de datos (`SQL Engine`) en lugar de en la memoria de .NET web.

## ⚡ Pasos a Seguir

1. **Consolidar Script**: Crear el archivo `SQL/Procedimientos_Vistas_Triggers.sql` con las copias literales del entorno desktop adaptadas.
2. **Inyectar SPs Operacionales**:
   - Módulo Empresas (`sp_ListarEmpresas`, `sp_BuscarEmpresa`, `sp_AgregarEmpresa`, etc.)
   - Módulo Empleados (`sp_ListarEmpleados`, `sp_VerificarCredencialRFID`, `sp_FiltrarEmpleadosSinAlmorzar`)
   - Módulo Registro (`sp_RegistrarComensal`, `sp_ListarComensalesPorServicio`)
3. **Inyectar Triggers y Vistas**:
   - Activar el Trigger `TR_ValidarRegistroUnico` sobre `Registros` para evitar insert loops fraudulentos de credenciales en el mismo día.
   - Activar la Vista `vw_EmpleadosSinAlmorzar`.
4. **Ejecución Local**: Ejecutar manualmente el bloque en SQL Server Management Studio para que los endpoints queden disponibles para la capa Negocio C#.

## 📁 Archivos a Crear y Modificar

| Archivo | Acción | Descripción |
|---|---|---|
| `SQL/Procedimientos_Vistas_Triggers.sql` | Crear | Script consolidado listo para ejecución en SSMS |

## 🚀 Mensaje de Commit

```
feat: crear stored procedures, vistas y triggers en SQL Server

- 33 stored procedures para todas las operaciones CRUD y reportes
- 2 vistas (EmpleadosSinAlmorzar, EmpresasConEmpleados)
- Trigger de validación de registro único
```
