# Guía Commit 49 — Seed data a escala real (~500 empleados)

## Commit
`68a4ccc` · `feat: expandir seed data a escala real (~500 empleados y registros del año en curso)`

---

## Contexto

El seed original tenía 60 empleados distribuidos en 6 empresas. Laboratorios Roemmers tiene ~500 empleados reales. Presentar el sistema con 60 empleados en una demo no transmite la escala real a la que está diseñado. Este commit expande el seed a 504 empleados y agrega registros históricos de todos los días hábiles de enero, marzo y abril de 2026.

---

## Estrategia de migración aditiva

El seed anterior usaba `HasData()` en el `DbContext`, lo que hace que EF Core lo gestione directamente en las migraciones. Agregar más datos con `HasData()` es seguro porque:

1. EF Core solo ejecuta los `INSERT` que aún no existen (compara por clave primaria).
2. No toca los datos ya existentes.
3. La migración `ExpandirSeedData` contiene exclusivamente `INSERT` nuevos, sin `DELETE` ni `UPDATE`.

---

## Volumen de datos

| Entidad | Antes | Después |
|---|---|---|
| Empleados | 60 | 504 |
| Empresas | 6 | 6 (sin cambio) |
| Empleados por empresa | ~10 | ~84 |
| Días de registro cubiertos | Febrero 2026 | Enero + Marzo + Abril 2026 |

---

## Proyecciones actualizadas

Con 504 empleados, las proyecciones de servicio se actualizaron a valores realistas:

- **Comedor:** 280–340 comensales por servicio
- **Quincho:** 160–200 comensales por servicio

Estos rangos reflejan el porcentaje real de asistencia observado en el comedor de Roemmers (no todos los empleados almuerzan en el comedor).

---

## Generación de nombres realistas

Los 444 empleados adicionales (74 por empresa) se generaron con nombres y apellidos argentinos comunes para que la demo sea verosímil. Las credenciales RFID siguen el patrón `RFID-[empresa]-[número]` con valores únicos garantizados.

---

## Herramientas de IA utilizadas

**Skill `/seed-data`** ejecutado para generar el bloque de `HasData()` con 444 empleados adicionales. Claude Code generó los INSERTs respetando el esquema de claves foráneas existente y verificó unicidad de `IdCredencial`. El skill `/migrate` ejecutó la migración y verificó que no hubiera errores de constraint.
