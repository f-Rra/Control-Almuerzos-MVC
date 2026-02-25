using Microsoft.Data.SqlClient;
using SCA_MVC.Data;
using SCA_MVC.ViewModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace SCA_MVC.Services
{
    public class EstadisticasNegocio : IEstadisticasNegocio
    {
        private readonly AccesoDatos _db;

        public EstadisticasNegocio(AccesoDatos db) => _db = db;

        public async Task<EstadisticasViewModel> ObtenerTodosAsync()
        {
            var vm = new EstadisticasViewModel();

            // ── Empleados ─────────────────────────────────────────────
            var empleados = await _db.ListarAsync(
                "sp_ObtenerEstadisticasEmpleados",
                CommandType.StoredProcedure,
                r =>
                {
                    vm.TotalEmpleados     = r.GetInt32(r.GetOrdinal("TotalRegistrados"));
                    vm.EmpleadosActivos   = r.GetInt32(r.GetOrdinal("TotalActivos"));
                    vm.EmpleadosInactivos = r.GetInt32(r.GetOrdinal("TotalInactivos"));
                    return true; // dummy para satisfacer Func<SqlDataReader, T>
                },
                Array.Empty<SqlParameter>());

            // ── Empresas ──────────────────────────────────────────────
            await _db.ListarAsync(
                "sp_ObtenerEstadisticasEmpresas",
                CommandType.StoredProcedure,
                r =>
                {
                    vm.TotalEmpresasActivas = r.GetInt32(r.GetOrdinal("TotalActivas"));
                    vm.EmpresasConEmpleados = r.GetInt32(r.GetOrdinal("TotalConEmpleados"));
                    vm.PromedioEmpleados    = Convert.ToDecimal(r["PromedioEmpleados"]);
                    return true;
                },
                Array.Empty<SqlParameter>());

            // ── Servicios ─────────────────────────────────────────────
            await _db.ListarAsync(
                "sp_ObtenerEstadisticasServicios",
                CommandType.StoredProcedure,
                r =>
                {
                    vm.ServiciosEsteMes  = r.GetInt32(r.GetOrdinal("ServiciosEsteMes"));
                    vm.ServiciosEsteAnio = r.GetInt32(r.GetOrdinal("ServiciosEsteAnio"));
                    vm.PromedioPorDia    = r.GetInt32(r.GetOrdinal("PromedioPorDia"));
                    return true;
                },
                Array.Empty<SqlParameter>());

            // ── Top 5 Empresas — mes actual ───────────────────────────
            var inicio = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
            var fin    = DateTime.Today;

            vm.TopEmpresas = await _db.ListarAsync(
                "sp_ObtenerTop5EmpresasPorAsistencias",
                CommandType.StoredProcedure,
                r => new TopEmpresaItem
                {
                    Ranking          = Convert.ToInt32(r["Ranking"]),
                    NombreEmpresa    = r.GetString(r.GetOrdinal("NombreEmpresa")),
                    TotalAsistencias = Convert.ToInt32(r["TotalAsistencias"]),
                    Porcentaje       = Convert.ToDecimal(r["Porcentaje"])
                },
                new[]
                {
                    new SqlParameter("@FechaInicio", inicio),
                    new SqlParameter("@FechaFin",    fin)
                });

            return vm;
        }
    }
}
