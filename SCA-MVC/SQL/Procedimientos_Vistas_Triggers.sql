
-- =============================================
-- PROCEDIMIENTOS DE LUGARES
-- =============================================

CREATE OR ALTER PROCEDURE sp_ListarLugares
AS
BEGIN
    SELECT 
        IdLugar, 
        Nombre
    FROM Lugares 
    WHERE Estado = 1
    ORDER BY Nombre;
END
GO

CREATE OR ALTER PROCEDURE sp_ObtenerLugarPorNombre
    @Nombre NVARCHAR(50)
AS
BEGIN
    SELECT 
        IdLugar,
        Nombre,
        Estado
    FROM Lugares 
    WHERE Nombre = @Nombre AND Estado = 1;
END
GO

-- =============================================
-- PROCEDIMIENTOS DE SERVICIOS
-- =============================================

CREATE OR ALTER PROCEDURE sp_ListarServicios
AS
BEGIN
    SELECT 
        s.IdServicio,
        s.Fecha,
        s.Proyeccion,
        s.DuracionMinutos,
        s.TotalComensales,
        s.TotalInvitados,
        l.Nombre as NombreLugar
    FROM Servicios s
    INNER JOIN Lugares l ON s.IdLugar = l.IdLugar
    ORDER BY s.Fecha DESC, l.Nombre;
END
GO


CREATE OR ALTER PROCEDURE sp_ObtenerServicioActivo
    @IdLugar INT
AS
BEGIN
    SELECT 
        IdServicio, 
        IdLugar, 
        Fecha,
        Proyeccion,
        DuracionMinutos,
        TotalComensales, 
        TotalInvitados
    FROM Servicios
    WHERE IdLugar = @IdLugar 
      AND Fecha = CAST(GETDATE() AS DATE)
      AND DuracionMinutos IS NULL; 
END
GO

CREATE OR ALTER PROCEDURE sp_ObtenerUltimoServicio
AS
BEGIN
    SELECT TOP 1
        s.IdServicio, 
        s.IdLugar, 
        l.Nombre as NombreLugar,
        s.Fecha,
        s.Proyeccion,
        s.DuracionMinutos,
        s.TotalComensales, 
        s.TotalInvitados
    FROM Servicios s
    INNER JOIN Lugares l ON s.IdLugar = l.IdLugar
    WHERE s.DuracionMinutos IS NOT NULL 
    ORDER BY s.Fecha DESC, s.IdServicio DESC;
END
GO

CREATE OR ALTER PROCEDURE sp_ObtenerServicioActivoSinLugar
AS
BEGIN
    SELECT TOP 1
        s.IdServicio, 
        s.IdLugar, 
        l.Nombre as NombreLugar,
        s.Fecha,
        s.Proyeccion,
        s.DuracionMinutos,
        s.TotalComensales, 
        s.TotalInvitados
    FROM Servicios s
    INNER JOIN Lugares l ON s.IdLugar = l.IdLugar
    WHERE s.DuracionMinutos IS NULL
    ORDER BY s.Fecha DESC, s.IdServicio DESC;
END
GO

CREATE OR ALTER PROCEDURE sp_IniciarServicio
    @IdLugar INT,
    @Proyeccion INT = NULL,
    @Fecha DATE = NULL,
    @Invitados INT = 0
AS
BEGIN
    DECLARE @FechaServicio DATE = ISNULL(@Fecha, GETDATE());

    -- Verificar si ya existe un servicio activo
    IF EXISTS (SELECT 1 FROM Servicios 
               WHERE IdLugar = @IdLugar AND DuracionMinutos IS NULL)
    BEGIN
        THROW 50001, 'Ya existe un servicio activo para este lugar', 1;
    END
    
    -- Insertar el nuevo servicio
    INSERT INTO Servicios (IdLugar, Fecha, Proyeccion, TotalInvitados)
    VALUES (@IdLugar, @FechaServicio, @Proyeccion, @Invitados);
    
    SELECT SCOPE_IDENTITY() AS IdServicio;
END
GO

CREATE OR ALTER PROCEDURE sp_FinalizarServicio
    @IdServicio INT,
    @TotalComensales INT,
    @TotalInvitados INT,
    @DuracionMinutos INT = NULL
AS
BEGIN
    UPDATE Servicios 
    SET TotalComensales = @TotalComensales,
        TotalInvitados = @TotalInvitados,
        DuracionMinutos = @DuracionMinutos
    WHERE IdServicio = @IdServicio;
END
GO

CREATE OR ALTER PROCEDURE sp_ListarServiciosPorFecha
    @FechaDesde DATE,
    @FechaHasta DATE
AS
BEGIN
    SELECT 
        s.IdServicio, 
        s.Fecha, 
        s.Proyeccion,
        s.DuracionMinutos,
        s.TotalComensales, 
        s.TotalInvitados,
        l.Nombre as Lugar,
        (s.TotalComensales + s.TotalInvitados) as Total
    FROM Servicios s
    INNER JOIN Lugares l ON s.IdLugar = l.IdLugar
    WHERE s.Fecha BETWEEN @FechaDesde AND @FechaHasta
    ORDER BY s.Fecha DESC, l.Nombre;
END
GO

CREATE OR ALTER PROCEDURE sp_ListarServiciosPorLugar
    @IdLugar INT,
    @FechaDesde DATE,
    @FechaHasta DATE
AS
BEGIN
    SELECT 
        s.IdServicio, 
        s.Fecha, 
        s.Proyeccion,
        s.DuracionMinutos,
        s.TotalComensales, 
        s.TotalInvitados,
        (s.TotalComensales + s.TotalInvitados) as Total
    FROM Servicios s
    WHERE s.IdLugar = @IdLugar
      AND s.Fecha BETWEEN @FechaDesde AND @FechaHasta
    ORDER BY s.Fecha DESC;
END
GO

CREATE OR ALTER PROCEDURE sp_ListarServiciosRango
    @FechaDesde DATE,
    @FechaHasta DATE,
    @IdLugar INT = NULL
AS
BEGIN
    SELECT 
        s.IdServicio, 
        s.Fecha, 
        s.Proyeccion,
        s.DuracionMinutos,
        s.TotalComensales, 
        s.TotalInvitados,
        l.Nombre as Lugar,
        (s.TotalComensales + s.TotalInvitados) as Total
    FROM Servicios s
    INNER JOIN Lugares l ON s.IdLugar = l.IdLugar
    WHERE s.Fecha BETWEEN @FechaDesde AND @FechaHasta
      AND (@IdLugar IS NULL OR s.IdLugar = @IdLugar)
    ORDER BY s.Fecha DESC, s.IdServicio DESC;
END
GO

-- Finalizar todos los servicios pendientes (sin importar la fecha)
CREATE OR ALTER PROCEDURE sp_FinalizarServiciosPendientes
AS
BEGIN
    UPDATE Servicios 
    SET DuracionMinutos = 60
    WHERE DuracionMinutos IS NULL;
    
    SELECT @@ROWCOUNT AS ServiciosFinalizados;
END
GO


-- =============================================
-- PROCEDIMIENTOS DE REGISTROS
-- =============================================

CREATE OR ALTER PROCEDURE sp_RegistrarEmpleado
    @IdEmpleado INT,
    @IdEmpresa INT,
    @IdServicio INT,
    @IdLugar INT
AS
BEGIN
    IF NOT EXISTS (SELECT 1 FROM Registros WHERE IdEmpleado = @IdEmpleado AND IdServicio = @IdServicio)
    BEGIN
        INSERT INTO Registros (IdEmpleado, IdEmpresa, IdServicio, IdLugar, Fecha, Hora)
        VALUES (@IdEmpleado, @IdEmpresa, @IdServicio, @IdLugar, 
                CAST(GETDATE() AS DATE), CAST(GETDATE() AS TIME));
    END
    ELSE
    BEGIN
        RETURN;
    END
END
GO

CREATE OR ALTER PROCEDURE sp_ListarRegistrosPorServicio
    @IdServicio INT
AS
BEGIN
    SELECT 
        r.IdRegistro, 
        r.Hora, 
        r.Fecha,
        e.Nombre + ' ' + e.Apellido as Empleado,
        emp.Nombre as Empresa,
        l.Nombre as Lugar
    FROM Registros r
    INNER JOIN Empleados e ON r.IdEmpleado = e.IdEmpleado
    INNER JOIN Empresas emp ON r.IdEmpresa = emp.IdEmpresa
    INNER JOIN Lugares l ON r.IdLugar = l.IdLugar
    WHERE r.IdServicio = @IdServicio
    ORDER BY r.IdRegistro DESC;
END
GO

CREATE OR ALTER PROCEDURE sp_VerificarEmpleadoRegistrado
    @IdEmpleado INT,
    @IdServicio INT
AS
BEGIN
    SELECT COUNT(*) as Registrado
    FROM Registros
    WHERE IdEmpleado = @IdEmpleado AND IdServicio = @IdServicio;
END
GO

CREATE OR ALTER PROCEDURE sp_ContarRegistrosPorServicio
    @IdServicio INT
AS
BEGIN
    SELECT COUNT(*) as TotalRegistros
    FROM Registros
    WHERE IdServicio = @IdServicio;
END
GO

-- =============================================
-- PROCEDIMIENTOS DE REPORTES
-- =============================================

CREATE OR ALTER PROCEDURE sp_AsistenciasPorEmpresas
    @FechaDesde DATE,
    @FechaHasta DATE,
    @IdLugar INT = NULL
AS
BEGIN
    SELECT 
        emp.Nombre as Empresa,
        COUNT(*) as TotalAsistencias
    FROM Registros r
    INNER JOIN Empresas emp ON r.IdEmpresa = emp.IdEmpresa
    WHERE r.Fecha BETWEEN @FechaDesde AND @FechaHasta
      AND (@IdLugar IS NULL OR r.IdLugar = @IdLugar)
    GROUP BY emp.Nombre, emp.IdEmpresa
    ORDER BY TotalAsistencias DESC;
END
GO

CREATE OR ALTER PROCEDURE sp_ReporteCoberturaVsProyeccion
    @FechaDesde DATE,
    @FechaHasta DATE,
    @IdLugar INT = NULL
AS
BEGIN
    SELECT 
        s.Fecha,
        l.Nombre as Lugar,
        ISNULL(s.Proyeccion, 0) as Proyeccion,
        (s.TotalComensales + s.TotalInvitados) as Atendidos,
        CASE WHEN ISNULL(s.Proyeccion, 0) > 0 
             THEN CAST((s.TotalComensales + s.TotalInvitados) * 100.0 / s.Proyeccion AS DECIMAL(10,2))
             ELSE NULL END as CoberturaPorcentaje,
        (s.TotalComensales + s.TotalInvitados) - ISNULL(s.Proyeccion, 0) as Diferencia
    FROM Servicios s
    INNER JOIN Lugares l ON s.IdLugar = l.IdLugar
    WHERE s.Fecha BETWEEN @FechaDesde AND @FechaHasta
      AND (@IdLugar IS NULL OR s.IdLugar = @IdLugar)
    ORDER BY s.Fecha DESC, l.Nombre;
END
GO

CREATE OR ALTER PROCEDURE sp_DistribucionPorDiaSemana
    @FechaDesde DATE,
    @FechaHasta DATE,
    @IdLugar INT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SET DATEFIRST 1;
    WITH Datos AS (
        SELECT 
            DATEPART(WEEKDAY, s.Fecha) as DiaNumero,
            (s.TotalComensales + s.TotalInvitados) as Total
        FROM Servicios s
        WHERE s.Fecha BETWEEN @FechaDesde AND @FechaHasta
          AND (@IdLugar IS NULL OR s.IdLugar = @IdLugar)
    )
    SELECT 
        DiaNumero as Orden,
        CASE DiaNumero
            WHEN 1 THEN 'Lunes'
            WHEN 2 THEN 'Martes'
            WHEN 3 THEN 'Miércoles'
            WHEN 4 THEN 'Jueves'
            WHEN 5 THEN 'Viernes'
            WHEN 6 THEN 'Sábado'
            WHEN 7 THEN 'Domingo'
            ELSE CAST(DiaNumero AS VARCHAR(10))
        END as Dia,
        SUM(Total) as Total
    FROM Datos
    GROUP BY DiaNumero
    ORDER BY Orden;
END
GO

-- =============================================
-- PROCEDIMIENTOS DE EMPLEADOS
-- =============================================

CREATE OR ALTER PROCEDURE sp_ListarEmpleados
AS
BEGIN
    SELECT 
        e.IdEmpleado, 
        e.Nombre, 
        e.Apellido, 
        e.IdCredencial, 
        emp.Nombre as Empresa, 
        emp.IdEmpresa,
        e.Estado
    FROM Empleados e
    INNER JOIN Empresas emp ON e.IdEmpresa = emp.IdEmpresa
    ORDER BY e.Estado DESC, e.Nombre, e.Apellido;
END
GO

CREATE OR ALTER PROCEDURE sp_BuscarEmpleadoPorCredencial
    @Credencial NVARCHAR(50)
AS
BEGIN
    SELECT 
        e.IdEmpleado, 
        e.Nombre, 
        e.Apellido, 
        e.IdCredencial,
        emp.Nombre as Empresa, 
        emp.IdEmpresa
    FROM Empleados e
    INNER JOIN Empresas emp ON e.IdEmpresa = emp.IdEmpresa
    WHERE e.IdCredencial = @Credencial AND e.Estado = 1;
END
GO

CREATE OR ALTER PROCEDURE sp_ListarEmpleadosPorEmpresa
    @IdEmpresa INT
AS
BEGIN
    SELECT 
        e.IdEmpleado, 
        e.Nombre, 
        e.Apellido, 
        e.IdCredencial
    FROM Empleados e
    WHERE e.IdEmpresa = @IdEmpresa AND e.Estado = 1
    ORDER BY e.Nombre, e.Apellido;
END
GO

CREATE OR ALTER PROCEDURE sp_EmpleadosSinAlmorzar
    @IdServicio INT
AS
BEGIN
    SELECT 
        e.IdEmpleado, 
        e.Nombre, 
        e.Apellido, 
        e.IdCredencial,
        e.NombreEmpresa as Empresa, 
        e.IdEmpresa,
        e.NombreCompleto
    FROM vw_EmpleadosSinAlmorzar e
    WHERE e.IdEmpleado NOT IN (
        SELECT IdEmpleado 
        FROM Registros 
        WHERE IdServicio = @IdServicio
    )
    ORDER BY e.Nombre, e.Apellido;
END
GO

CREATE OR ALTER PROCEDURE sp_FiltrarEmpleadosSinAlmorzar
    @IdServicio INT,
    @IdEmpresa INT = NULL,        
    @Nombre NVARCHAR(100) = NULL  
AS
BEGIN
    SELECT 
        e.IdEmpleado, 
        e.Nombre, 
        e.Apellido, 
        e.IdCredencial,
        e.NombreEmpresa as Empresa, 
        e.IdEmpresa,
        e.NombreCompleto
    FROM vw_EmpleadosSinAlmorzar e
    WHERE e.IdEmpleado NOT IN (
        SELECT IdEmpleado 
        FROM Registros 
        WHERE IdServicio = @IdServicio
    )
    AND (@IdEmpresa IS NULL OR e.IdEmpresa = @IdEmpresa)
    AND (@Nombre IS NULL OR e.NombreCompleto LIKE '%' + @Nombre + '%')
    ORDER BY e.Nombre, e.Apellido;
END
GO

CREATE OR ALTER PROCEDURE sp_AgregarEmpleado
    @IdCredencial NVARCHAR(50),
    @Nombre NVARCHAR(50),
    @Apellido NVARCHAR(50),
    @IdEmpresa INT,
    @Estado BIT = 1
AS
BEGIN
    INSERT INTO Empleados (IdCredencial, Nombre, Apellido, IdEmpresa, Estado)
    VALUES (@IdCredencial, @Nombre, @Apellido, @IdEmpresa, @Estado);
    
    SELECT CAST(SCOPE_IDENTITY() AS INT) as IdEmpleado;
END
GO

CREATE OR ALTER PROCEDURE sp_ModificarEmpleado
    @IdEmpleado INT,
    @IdCredencial NVARCHAR(50),
    @Nombre NVARCHAR(50),
    @Apellido NVARCHAR(50),
    @IdEmpresa INT,
    @Estado BIT
AS
BEGIN
    UPDATE Empleados 
    SET IdCredencial = @IdCredencial,
        Nombre = @Nombre,
        Apellido = @Apellido,
        IdEmpresa = @IdEmpresa,
        Estado = @Estado
    WHERE IdEmpleado = @IdEmpleado;
END
GO

CREATE OR ALTER PROCEDURE sp_DesactivarEmpleado
    @IdEmpleado INT
AS
BEGIN
    UPDATE Empleados 
    SET Estado = 0 
    WHERE IdEmpleado = @IdEmpleado;
END
GO

CREATE OR ALTER PROCEDURE sp_VerificarCredencial
    @IdCredencial NVARCHAR(50)
AS
BEGIN
    SELECT COUNT(*) as Registrado
    FROM Empleados
    WHERE IdCredencial = @IdCredencial;
END
GO

CREATE OR ALTER PROCEDURE sp_BuscarEmpleadoPorId
    @IdEmpleado INT
AS
BEGIN
    SELECT 
        e.IdEmpleado, 
        e.IdCredencial, 
        e.Nombre, 
        e.Apellido, 
        e.Estado, 
        emp.IdEmpresa, 
        emp.Nombre as NombreEmpresa
    FROM Empleados e
    INNER JOIN Empresas emp ON e.IdEmpresa = emp.IdEmpresa
    WHERE e.IdEmpleado = @IdEmpleado;
END
GO

-- =============================================
-- PROCEDIMIENTOS DE EMPRESAS
-- =============================================

CREATE OR ALTER PROCEDURE sp_ListarEmpresas
AS
BEGIN
    SELECT IdEmpresa, Nombre
    FROM Empresas
    WHERE Estado = 1
    ORDER BY Nombre;
END
GO

CREATE OR ALTER PROCEDURE sp_AgregarEmpresa
    @Nombre NVARCHAR(100),
    @Estado BIT = 1
AS
BEGIN
    INSERT INTO Empresas (Nombre, Estado)
    VALUES (@Nombre, @Estado);
    
    SELECT CAST(SCOPE_IDENTITY() AS INT) as IdEmpresa;
END
GO

CREATE OR ALTER PROCEDURE sp_ModificarEmpresa
    @IdEmpresa INT,
    @Nombre NVARCHAR(100),
    @Estado BIT
AS
BEGIN
    UPDATE Empresas 
    SET Nombre = @Nombre,
        Estado = @Estado
    WHERE IdEmpresa = @IdEmpresa;
END
GO

CREATE OR ALTER PROCEDURE sp_DesactivarEmpresa
    @IdEmpresa INT
AS
BEGIN
    UPDATE Empresas 
    SET Estado = 0 
    WHERE IdEmpresa = @IdEmpresa;
END
GO

CREATE OR ALTER PROCEDURE sp_BuscarEmpresaPorId
    @IdEmpresa INT
AS
BEGIN
    SELECT IdEmpresa, Nombre, Estado
    FROM Empresas
    WHERE IdEmpresa = @IdEmpresa;
END
GO

CREATE OR ALTER PROCEDURE sp_ListarEmpresasConEmpleados
AS
BEGIN
    SELECT 
        IdEmpresa,
        Empresa as Nombre,
        Estado,
        CantidadEmpleados
    FROM vw_EmpresasConEmpleados
    ORDER BY Empresa;
END
GO

-- =============================================
-- VISTAS
-- =============================================

CREATE OR ALTER VIEW vw_EmpleadosSinAlmorzar AS
SELECT 
    e.IdEmpleado,
    e.Nombre,
    e.Apellido,
    e.IdCredencial,
    e.IdEmpresa,
    emp.Nombre as NombreEmpresa,
    CONCAT(e.Nombre, ' ', e.Apellido) as NombreCompleto
FROM Empleados e
INNER JOIN Empresas emp ON e.IdEmpresa = emp.IdEmpresa
WHERE e.Estado = 1;
GO

CREATE OR ALTER VIEW vw_EmpresasConEmpleados AS
SELECT 
    emp.IdEmpresa,
    emp.Nombre as Empresa,
    emp.Estado,
    COUNT(e.IdEmpleado) as CantidadEmpleados
FROM Empresas emp
LEFT JOIN Empleados e ON emp.IdEmpresa = e.IdEmpresa AND e.Estado = 1
GROUP BY emp.IdEmpresa, emp.Nombre, emp.Estado;
GO

-- =============================================
-- TRIGGERS
-- =============================================

CREATE OR ALTER TRIGGER TR_ValidarRegistroUnico
ON Registros
INSTEAD OF INSERT
AS
BEGIN
    SET NOCOUNT ON;
    
    IF NOT EXISTS (
        SELECT 1 FROM inserted i
        INNER JOIN Registros r ON i.IdEmpleado = r.IdEmpleado 
                              AND i.IdServicio = r.IdServicio
    )
    BEGIN
        INSERT INTO Registros (IdEmpleado, IdEmpresa, IdServicio, IdLugar, Fecha, Hora)
        SELECT IdEmpleado, IdEmpresa, IdServicio, IdLugar, Fecha, Hora
        FROM inserted;
    END
    ELSE
    BEGIN
        RAISERROR('El empleado ya está registrado en este servicio', 16, 1);
    END
END
GO

CREATE OR ALTER PROCEDURE sp_ObtenerRegistrosPorEmpresaYFecha
    @IdEmpresa INT,
    @FechaInicio DATE,
    @FechaFin DATE
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        r.IdRegistro,
        r.IdEmpleado,
        r.IdEmpresa,
        r.IdServicio,
        r.IdLugar,
        r.Fecha,
        r.Hora,
        CONCAT(e.Nombre, ' ', e.Apellido) as NombreEmpleado,
        emp.Nombre as NombreEmpresa
    FROM Registros r
    INNER JOIN Empleados e ON r.IdEmpleado = e.IdEmpleado
    INNER JOIN Empresas emp ON r.IdEmpresa = emp.IdEmpresa
    WHERE r.IdEmpresa = @IdEmpresa
      AND r.Fecha >= @FechaInicio
      AND r.Fecha <= @FechaFin
    ORDER BY r.Fecha DESC, r.Hora DESC;
END
GO

-- =============================================
-- PROCEDIMIENTOS DE ESTADÍSTICAS
-- =============================================

CREATE OR ALTER PROCEDURE sp_ObtenerEstadisticasEmpleados
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        COUNT(*) as TotalRegistrados,
        SUM(CASE WHEN Estado = 1 THEN 1 ELSE 0 END) as TotalActivos,
        SUM(CASE WHEN Estado = 0 THEN 1 ELSE 0 END) as TotalInactivos
    FROM Empleados;
END
GO


CREATE OR ALTER PROCEDURE sp_ObtenerEstadisticasEmpresas
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @TotalActivas INT;
    DECLARE @TotalConEmpleados INT;
    DECLARE @PromedioEmpleados DECIMAL(10,2);
    
    -- Total empresas activas
    SELECT @TotalActivas = COUNT(*) 
    FROM Empresas 
    WHERE Estado = 1;
    
    -- Total empresas con al menos un empleado
    SELECT @TotalConEmpleados = COUNT(DISTINCT IdEmpresa)
    FROM Empleados
    WHERE IdEmpresa IN (SELECT IdEmpresa FROM Empresas WHERE Estado = 1);
    
    -- Promedio de empleados por empresa activa
    IF @TotalActivas > 0
    BEGIN
        SELECT @PromedioEmpleados = CAST(COUNT(*) AS DECIMAL(10,2)) / @TotalActivas
        FROM Empleados
        WHERE IdEmpresa IN (SELECT IdEmpresa FROM Empresas WHERE Estado = 1);
    END
    ELSE
    BEGIN
        SET @PromedioEmpleados = 0;
    END
    
    SELECT 
        @TotalActivas as TotalActivas,
        @TotalConEmpleados as TotalConEmpleados,
        @PromedioEmpleados as PromedioEmpleados;
END
GO


CREATE OR ALTER PROCEDURE sp_ObtenerEstadisticasServicios
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @ServiciosEsteMes INT;
    DECLARE @ServiciosEsteAnio INT;
    DECLARE @PromedioPorDia INT;
    DECLARE @PrimerDiaMes DATE;
    DECLARE @DiasTranscurridos INT;
    
    SET @PrimerDiaMes = DATEFROMPARTS(YEAR(GETDATE()), MONTH(GETDATE()), 1);
    SET @DiasTranscurridos = DATEDIFF(DAY, @PrimerDiaMes, GETDATE()) + 1;
    
    -- Servicios del mes actual
    SELECT @ServiciosEsteMes = COUNT(*)
    FROM Servicios
    WHERE YEAR(Fecha) = YEAR(GETDATE())
      AND MONTH(Fecha) = MONTH(GETDATE());
    
    -- Servicios del año actual
    SELECT @ServiciosEsteAnio = COUNT(*)
    FROM Servicios
    WHERE YEAR(Fecha) = YEAR(GETDATE());
    
    -- Promedio por día
    IF @DiasTranscurridos > 0
        SET @PromedioPorDia = ROUND(CAST(@ServiciosEsteMes AS FLOAT) / @DiasTranscurridos, 0);
    ELSE
        SET @PromedioPorDia = 0;
    
    SELECT 
        @ServiciosEsteMes as ServiciosEsteMes,
        @ServiciosEsteAnio as ServiciosEsteAnio,
        @PromedioPorDia as PromedioPorDia;
END
GO


CREATE OR ALTER PROCEDURE sp_ObtenerAsistenciasTendencias
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @AsistenciasTotales INT;
    DECLARE @AsistenciasEmpleados INT;
    DECLARE @AsistenciasInvitados INT;
    DECLARE @PromedioDiario INT;
    DECLARE @CoberturaPromedio DECIMAL(5,2);
    DECLARE @DuracionPromedio INT;
    DECLARE @PrimerDiaMes DATE;
    DECLARE @DiasTranscurridos INT;
    
    SET @PrimerDiaMes = DATEFROMPARTS(YEAR(GETDATE()), MONTH(GETDATE()), 1);
    SET @DiasTranscurridos = DATEDIFF(DAY, @PrimerDiaMes, GETDATE()) + 1;
    
    -- Asistencias de empleados (registros)
    SELECT @AsistenciasEmpleados = COUNT(*)
    FROM Registros
    WHERE YEAR(Fecha) = YEAR(GETDATE())
      AND MONTH(Fecha) = MONTH(GETDATE());
    
    -- Asistencias de invitados (desde Servicios)
    SELECT @AsistenciasInvitados = ISNULL(SUM(TotalInvitados), 0)
    FROM Servicios
    WHERE YEAR(Fecha) = YEAR(GETDATE())
      AND MONTH(Fecha) = MONTH(GETDATE());
    
    -- Asistencias totales
    SET @AsistenciasTotales = @AsistenciasEmpleados + @AsistenciasInvitados;
    
    -- Promedio diario
    IF @DiasTranscurridos > 0
        SET @PromedioDiario = ROUND(CAST(@AsistenciasTotales AS FLOAT) / @DiasTranscurridos, 0);
    ELSE
        SET @PromedioDiario = 0;
    
    -- Cobertura promedio vs proyección
    SELECT @CoberturaPromedio = AVG(CAST(s.TotalComensales AS FLOAT) / NULLIF(s.Proyeccion, 0) * 100)
    FROM Servicios s
    WHERE YEAR(s.Fecha) = YEAR(GETDATE())
      AND MONTH(s.Fecha) = MONTH(GETDATE())
      AND s.Proyeccion IS NOT NULL
      AND s.Proyeccion > 0;
    
    -- Duración promedio de servicio
    SELECT @DuracionPromedio = AVG(DuracionMinutos)
    FROM Servicios
    WHERE YEAR(Fecha) = YEAR(GETDATE())
      AND MONTH(Fecha) = MONTH(GETDATE())
      AND DuracionMinutos IS NOT NULL;
    
    -- Valores por defecto si son NULL
    SET @CoberturaPromedio = ISNULL(@CoberturaPromedio, 0);
    SET @DuracionPromedio = ISNULL(@DuracionPromedio, 0);
    
    SELECT 
        @AsistenciasTotales as AsistenciasTotales,
        @AsistenciasEmpleados as AsistenciasEmpleados,
        @AsistenciasInvitados as AsistenciasInvitados,
        @PromedioDiario as PromedioDiario,
        @CoberturaPromedio as CoberturaPromedio,
        @DuracionPromedio as DuracionPromedio;
END
GO


CREATE OR ALTER PROCEDURE sp_ObtenerTop5EmpresasPorAsistencias
    @FechaInicio DATE,
    @FechaFin DATE
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @TotalGeneral INT;
    
    -- Obtener total general de asistencias en el periodo
    SELECT @TotalGeneral = COUNT(*)
    FROM Registros
    WHERE Fecha >= @FechaInicio
      AND Fecha <= @FechaFin;
    
    -- Obtener top 5 con ranking
    SELECT TOP 5
        ROW_NUMBER() OVER (ORDER BY COUNT(*) DESC) as Ranking,
        emp.Nombre as NombreEmpresa,
        COUNT(*) as TotalAsistencias,
        CASE 
            WHEN @TotalGeneral > 0 THEN ROUND(CAST(COUNT(*) AS FLOAT) / @TotalGeneral * 100, 2)
            ELSE 0 
        END as Porcentaje
    FROM Registros r
    INNER JOIN Empresas emp ON r.IdEmpresa = emp.IdEmpresa
    WHERE r.Fecha >= @FechaInicio
      AND r.Fecha <= @FechaFin
    GROUP BY emp.IdEmpresa, emp.Nombre
    ORDER BY TotalAsistencias DESC;
END
GO


-- =============================================
-- PROCEDIMIENTOS DE CONFIGURACIÓN
-- =============================================

CREATE OR ALTER PROCEDURE sp_ObtenerInfoBaseDatos
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        DB_NAME() as NombreBaseDatos,
        CAST(SUM(size) * 8.0 / 1024 AS DECIMAL(10,2)) as TamañoMB,
        (SELECT create_date FROM sys.databases WHERE name = DB_NAME()) as FechaCreacion,
        GETDATE() as UltimaActualizacion
    FROM sys.master_files
    WHERE database_id = DB_ID();
END
GO


CREATE OR ALTER PROCEDURE sp_ObtenerUltimoRespaldo
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT TOP 1
        backup_start_date as FechaRespaldo,
        physical_device_name as RutaArchivo,
        CAST(backup_size / 1024.0 / 1024.0 AS DECIMAL(10,2)) as TamañoMB
    FROM msdb.dbo.backupset bs
    INNER JOIN msdb.dbo.backupmediafamily bmf ON bs.media_set_id = bmf.media_set_id
    WHERE database_name = DB_NAME()
      AND type = 'D' -- Full backup
    ORDER BY backup_start_date DESC;
END
GO


CREATE OR ALTER PROCEDURE sp_CrearRespaldo
    @RutaDestino NVARCHAR(500)
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @NombreDB NVARCHAR(128) = DB_NAME();
    DECLARE @SQL NVARCHAR(MAX);
    
    -- Verificar que la ruta tenga extensión .bak
    IF RIGHT(@RutaDestino, 4) != '.bak'
        SET @RutaDestino = @RutaDestino + '.bak';
    
    -- Construir comando BACKUP
    SET @SQL = 'BACKUP DATABASE [' + @NombreDB + '] ' +
               'TO DISK = ''' + @RutaDestino + ''' ' +
               'WITH FORMAT, ' +
               'MEDIANAME = ''SQLServerBackups'', ' +
               'NAME = ''Full Backup of ' + @NombreDB + ''';';
    
    -- Ejecutar backup
    EXEC sp_executesql @SQL;
    
    -- Retornar información del backup creado
    SELECT 
        @RutaDestino as RutaArchivo,
        GETDATE() as FechaRespaldo,
        'Respaldo creado exitosamente' as Mensaje;
END
GO


CREATE OR ALTER PROCEDURE sp_RestaurarRespaldo
    @RutaArchivo NVARCHAR(500)
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @NombreDB NVARCHAR(128) = DB_NAME();
    DECLARE @SQL NVARCHAR(MAX);
    
    -- Usar contexto dinámico desde master
    -- Primero cambiar a master y luego restaurar
    SET @SQL = '
        USE master;
        
        -- Cerrar todas las conexiones activas
        ALTER DATABASE [' + @NombreDB + '] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
        
        -- Restaurar base de datos
        RESTORE DATABASE [' + @NombreDB + '] 
        FROM DISK = ''' + @RutaArchivo + ''' 
        WITH REPLACE;
        
        -- Volver a multi usuario
        ALTER DATABASE [' + @NombreDB + '] SET MULTI_USER;
    ';
    
    -- Ejecutar restore
    EXEC sp_executesql @SQL;
    
    -- Retornar información
    SELECT 
        @RutaArchivo as RutaArchivo,
        GETDATE() as FechaRestauracion,
        'Respaldo restaurado exitosamente' as Mensaje;
END
GO

-- =============================================
-- PROCEDIMIENTOS DE FILTRADO OPTIMIZADO
-- =============================================

-- Filtrar empleados por nombre, apellido, credencial y empresa
CREATE OR ALTER PROCEDURE sp_FiltrarEmpleados
    @Filtro NVARCHAR(100) = NULL,
    @IdEmpresa INT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        e.IdEmpleado,
        e.IdCredencial,
        e.Nombre,
        e.Apellido,
        e.IdEmpresa,
        emp.Nombre as NombreEmpresa,
        e.Estado
    FROM Empleados e
    INNER JOIN Empresas emp ON e.IdEmpresa = emp.IdEmpresa
    WHERE 
        (@Filtro IS NULL OR 
         e.Nombre LIKE '%' + @Filtro + '%' OR 
         e.Apellido LIKE '%' + @Filtro + '%' OR
         e.IdCredencial LIKE '%' + @Filtro + '%')
        AND (@IdEmpresa IS NULL OR e.IdEmpresa = @IdEmpresa)
        AND e.Estado = 1
    ORDER BY e.Apellido, e.Nombre;
END
GO

-- Filtrar empresas por nombre
CREATE OR ALTER PROCEDURE sp_FiltrarEmpresas
    @Filtro NVARCHAR(100) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        e.IdEmpresa,
        e.Nombre,
        e.Estado,
        (SELECT COUNT(*) FROM Empleados WHERE IdEmpresa = e.IdEmpresa AND Estado = 1) as CantidadEmpleados
    FROM Empresas e
    WHERE (@Filtro IS NULL OR e.Nombre LIKE '%' + @Filtro + '%')
        AND e.Estado = 1
    ORDER BY e.Nombre;
END
GO








