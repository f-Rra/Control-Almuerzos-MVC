using Microsoft.Data.SqlClient;
using SCA_MVC.Data;
using SCA_MVC.Data.Mappers;
using SCA_MVC.Models;
using System.Data;

namespace SCA_MVC.Services
{
    public class RegistroNegocio : IRegistroNegocio
    {
        private readonly AccesoDatos _accesoDatos;

        public RegistroNegocio(AccesoDatos accesoDatos)
        {
            _accesoDatos = accesoDatos;
        }

        public Task RegistrarAsync(int idEmpleado, int idEmpresa, int idServicio, int idLugar)
        {
            var parametros = new[]
            {
                new SqlParameter("@IdEmpleado", idEmpleado),
                new SqlParameter("@IdEmpresa", idEmpresa),
                new SqlParameter("@IdServicio", idServicio),
                new SqlParameter("@IdLugar", idLugar)
            };

            return _accesoDatos.EjecutarAsync("sp_RegistrarEmpleado", CommandType.StoredProcedure, parametros);
        }

        public Task<List<Registro>> ListarPorServicioAsync(int idServicio)
        {
            var parametros = new[]
            {
                new SqlParameter("@IdServicio", idServicio)
            };

            return _accesoDatos.ListarAsync("sp_ListarRegistrosPorServicio", CommandType.StoredProcedure, MapRegistroListado, parametros);
        }

        public async Task<bool> YaRegistradoAsync(int idEmpleado, int idServicio)
        {
            var parametros = new[]
            {
                new SqlParameter("@IdEmpleado", idEmpleado),
                new SqlParameter("@IdServicio", idServicio)
            };

            var resultado = await _accesoDatos.EscalarAsync("sp_VerificarEmpleadoRegistrado", CommandType.StoredProcedure, parametros);
            return Convert.ToInt32(resultado) > 0;
        }

        public async Task<int> ContarAsync(int idServicio)
        {
            var parametros = new[]
            {
                new SqlParameter("@IdServicio", idServicio)
            };

            var resultado = await _accesoDatos.EscalarAsync("sp_ContarRegistrosPorServicio", CommandType.StoredProcedure, parametros);
            return Convert.ToInt32(resultado);
        }

        public Task<List<Registro>> PorEmpresaYFechaAsync(int idEmpresa, DateTime fechaInicio, DateTime fechaFin)
        {
            var parametros = new[]
            {
                new SqlParameter("@IdEmpresa", idEmpresa),
                new SqlParameter("@FechaInicio", fechaInicio.Date),
                new SqlParameter("@FechaFin", fechaFin.Date)
            };

            return _accesoDatos.ListarAsync("sp_ObtenerRegistrosPorEmpresaYFecha", CommandType.StoredProcedure, RegistroMapper.Map, parametros);
        }

        private static Registro MapRegistroListado(SqlDataReader reader)
        {
            return new Registro
            {
                IdRegistro    = DbMapper.GetInt32(reader, nameof(Registro.IdRegistro)),
                Fecha         = DbMapper.GetDateTime(reader, nameof(Registro.Fecha)),
                Hora          = DbMapper.GetTimeSpan(reader, nameof(Registro.Hora)),
                // El SP devuelve strings ya combinados, los mapeamos directamente
                NombreEmpleado = DbMapper.GetString(reader, "Empleado"),
                NombreEmpresa  = DbMapper.GetString(reader, "Empresa")
            };
        }
    }
}