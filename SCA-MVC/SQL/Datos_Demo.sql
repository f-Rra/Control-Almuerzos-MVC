-- =============================================================================
-- Datos de demostración de SCA-MVC
--
-- Reemplaza los servicios y registros que vienen de la semilla (enero a abril de
-- 2026, con proyecciones que no tienen sentido) por servicios de los últimos 60
-- días hábiles anteriores a hoy, así el dashboard, los reportes y las estadísticas
-- se ven con actividad reciente.
--
-- QUÉ HACE
--   - Borra TODOS los servicios y registros y los vuelve a generar.
--   - Comedor: proyección de unos 300 comensales y asistencia de 94 % en promedio.
--   - Quincho: proyección de unos 135 y asistencia de 89 % en promedio.
--     Según el día, la cobertura va de 72 a 110 %: hay días flojos y días que se pasan.
--   - Nadie almuerza en los dos lugares el mismo día.
--   - No hay servicio de hoy: queda libre para iniciar uno en vivo.
--   - No toca empresas, empleados, lugares, lectores ni usuarios.
--
-- CÓMO CORRERLO
--   Contra la base de la demo (Azure SQL o local) con SSMS o Azure Data Studio.
--   No usa GO, así que también corre en el editor de consultas del portal de Azure.
--   Se puede correr las veces que haga falta: cada vez corre los días hasta hoy.
--
-- NO LO CORRAS contra una base con datos reales: borra los registros de asistencia.
-- =============================================================================

SET NOCOUNT ON;
SET XACT_ABORT ON;
SET QUOTED_IDENTIFIER ON; -- el índice filtrado de Registros lo exige (sqlcmd lo trae apagado)

BEGIN TRANSACTION;

DECLARE @hoy date = CAST(SYSDATETIMEOFFSET() AT TIME ZONE 'Argentina Standard Time' AS date);

-- 1. Borrar lo anterior ---------------------------------------------------------
DELETE FROM Registros;
DELETE FROM Servicios;
DBCC CHECKIDENT ('Registros', RESEED, 0) WITH NO_INFOMSGS;
DBCC CHECKIDENT ('Servicios', RESEED, 0) WITH NO_INFOMSGS;

-- 2. Días hábiles de los últimos 60 días (sin hoy) --------------------------------
-- El 1/1/1900 fue lunes: el resto de la división por 7 da 5 el sábado y 6 el domingo.
CREATE TABLE #Dias (Fecha date NOT NULL PRIMARY KEY, Orden int NOT NULL);

WITH Atras AS (
    SELECT 1 AS n
    UNION ALL
    SELECT n + 1 FROM Atras WHERE n < 60
)
INSERT INTO #Dias (Fecha, Orden)
SELECT d, ROW_NUMBER() OVER (ORDER BY d)
FROM (SELECT DATEADD(DAY, -n, @hoy) AS d FROM Atras) x
WHERE DATEDIFF(DAY, '19000101', d) % 7 NOT IN (5, 6);

-- 3. Servicios --------------------------------------------------------------------
-- Cada servicio se arma con valores que salen de la fecha, así que son siempre los
-- mismos para un día dado. HoraInicio va en UTC (Argentina es UTC-3).
INSERT INTO Servicios (IdLugar, Fecha, HoraInicio, Proyeccion, DuracionMinutos, TotalComensales, TotalInvitados)
SELECT l.IdLugar,
       d.Fecha,
       DATEADD(HOUR, CASE l.IdLugar WHEN 1 THEN 15 ELSE 16 END, CAST(d.Fecha AS datetime2)),
       CASE l.IdLugar
            WHEN 1 THEN 290 + ABS(CHECKSUM(d.Orden, 1)) % 21
            ELSE        125 + ABS(CHECKSUM(d.Orden, 2)) % 21
       END,
       CASE l.IdLugar
            WHEN 1 THEN 55 + ABS(CHECKSUM(d.Orden, 3)) % 21
            ELSE        40 + ABS(CHECKSUM(d.Orden, 4)) % 21
       END,
       0,
       CASE l.IdLugar
            WHEN 1 THEN ABS(CHECKSUM(d.Orden, 5)) % 8
            ELSE        ABS(CHECKSUM(d.Orden, 6)) % 5
       END
FROM #Dias d
CROSS JOIN (SELECT IdLugar FROM Lugares WHERE IdLugar IN (1, 2)) l;

-- 4. Registros --------------------------------------------------------------------
-- Cada empleado activo tiene un número del 0 al 99 por día:
--   Comedor: almuerza si ese número es menor que el umbral del día (53 a 58).
--   Quincho: almuerza si el número está entre 60 y 60 + el 40 % del umbral + 2.
-- Así los dos grupos no se pisan y hay unos 270 a 290 en el comedor y unos 115 a 125 en el quincho.
INSERT INTO Registros (IdEmpleado, IdEmpresa, IdServicio, IdLugar, Fecha, Hora)
SELECT e.IdEmpleado,
       e.IdEmpresa,
       s.IdServicio,
       s.IdLugar,
       s.Fecha,
       CASE s.IdLugar
            WHEN 1 THEN DATEADD(MINUTE, 5  + ABS(CHECKSUM(e.IdEmpleado, d.Orden, 7)) % 55, CAST('12:00' AS time))
            ELSE        DATEADD(MINUTE, 10 + ABS(CHECKSUM(e.IdEmpleado, d.Orden, 8)) % 45, CAST('13:00' AS time))
       END
FROM Servicios s
JOIN #Dias d ON d.Fecha = s.Fecha
CROSS JOIN (SELECT IdEmpleado, IdEmpresa FROM Empleados WHERE Estado = 1) e
CROSS APPLY (SELECT ABS(CHECKSUM(e.IdEmpleado, d.Orden)) % 100 AS Num,
                    53 + ABS(CHECKSUM(d.Orden, 9)) % 6 AS Umbral) h
WHERE (s.IdLugar = 1 AND h.Num < h.Umbral)
   OR (s.IdLugar = 2 AND h.Num >= 60 AND h.Num < 60 + h.Umbral * 2 / 5 + 2);

-- 5. Totales de cada servicio = los registros que quedaron ------------------------
UPDATE s
SET TotalComensales = (SELECT COUNT(*) FROM Registros r WHERE r.IdServicio = s.IdServicio)
FROM Servicios s;

COMMIT TRANSACTION;

DROP TABLE #Dias;

-- 6. Resumen para comprobar -------------------------------------------------------
SELECT l.Nombre AS Lugar,
       COUNT(*)                                          AS Servicios,
       MIN(s.Fecha)                                      AS Desde,
       MAX(s.Fecha)                                      AS Hasta,
       AVG(s.Proyeccion)                                 AS ProyeccionPromedio,
       AVG(s.TotalComensales)                            AS ComensalesPromedio,
       CAST(100.0 * SUM(s.TotalComensales) / SUM(s.Proyeccion) AS decimal(5, 1)) AS CoberturaPorCiento
FROM Servicios s
JOIN Lugares l ON l.IdLugar = s.IdLugar
GROUP BY l.Nombre;
