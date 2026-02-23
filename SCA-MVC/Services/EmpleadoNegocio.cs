using Microsoft.Data.SqlClient;
using SCA_MVC.Data;
using SCA_MVC.Data.Mappers;
using SCA_MVC.Models;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SCA_MVC.Services
{
    public class EmpleadoNegocio : IEmpleadoNegocio
    {
        #region Atributos y Constructor
        private readonly AccesoDatos _accesoDatos;

        public EmpleadoNegocio(AccesoDatos accesoDatos)
        {
            _accesoDatos = accesoDatos;
        }
        #endregion

        #region Métodos de Lectura
        public Task<List<Empleado>> ListarAsync()
        {
            return _accesoDatos.ListarAsync("sp_ListarEmpleados", CommandType.StoredProcedure, EmpleadoMapper.Map);
        }

        public Task<Empleado?> BuscarPorCredencialAsync(string idCredencial)
        {
            var parametros = new[]
            {
                new SqlParameter("@Credencial", idCredencial)
            };

            return _accesoDatos.ObtenerPrimeroAsync("sp_BuscarEmpleadoPorCredencial", CommandType.StoredProcedure, MapEmpleadoSinEstado, parametros);
        }

        public Task<Empleado?> BuscarPorIdAsync(int idEmpleado)
        {
            var parametros = new[]
            {
                new SqlParameter("@IdEmpleado", idEmpleado)
            };

            return _accesoDatos.ObtenerPrimeroAsync("sp_BuscarEmpleadoPorId", CommandType.StoredProcedure, MapEmpleadoDetalle, parametros);
        }

        public async Task<bool> ExisteCredencialAsync(string idCredencial)
        {
            var parametros = new[]
            {
                new SqlParameter("@IdCredencial", idCredencial)
            };

            var resultado = await _accesoDatos.EscalarAsync("sp_VerificarCredencial", CommandType.StoredProcedure, parametros);
            return Convert.ToInt32(resultado) > 0;
        }
        #endregion

        #region Métodos de Escritura (CUD)
        public async Task<int> AgregarAsync(Empleado empleado)
        {
            var parametros = new[]
            {
                new SqlParameter("@IdCredencial", empleado.IdCredencial),
                new SqlParameter("@Nombre", empleado.Nombre),
                new SqlParameter("@Apellido", empleado.Apellido),
                new SqlParameter("@IdEmpresa", empleado.IdEmpresa),
                new SqlParameter("@Estado", empleado.Estado)
            };

            var id = await _accesoDatos.EscalarAsync("sp_AgregarEmpleado", CommandType.StoredProcedure, parametros);
            return Convert.ToInt32(id);
        }

        public Task ModificarAsync(Empleado empleado)
        {
            var parametros = new[]
            {
                new SqlParameter("@IdEmpleado", empleado.IdEmpleado),
                new SqlParameter("@IdCredencial", empleado.IdCredencial),
                new SqlParameter("@Nombre", empleado.Nombre),
                new SqlParameter("@Apellido", empleado.Apellido),
                new SqlParameter("@IdEmpresa", empleado.IdEmpresa),
                new SqlParameter("@Estado", empleado.Estado)
            };

            return _accesoDatos.EjecutarAsync("sp_ModificarEmpleado", CommandType.StoredProcedure, parametros);
        }

        public Task EliminarAsync(int idEmpleado)
        {
            var parametros = new[]
            {
                new SqlParameter("@IdEmpleado", idEmpleado)
            };

            return _accesoDatos.EjecutarAsync("sp_DesactivarEmpleado", CommandType.StoredProcedure, parametros);
        }
        #endregion

        #region Métodos de Filtrado y Registro Manual
        public Task<List<Empleado>> FiltrarEmpleadosAsync(string? filtro, int? idEmpresa)
        {
            var parametros = new[]
            {
                new SqlParameter("@Filtro", (object?)filtro ?? DBNull.Value),
                new SqlParameter("@IdEmpresa", (object?)idEmpresa ?? DBNull.Value)
            };

            return _accesoDatos.ListarAsync("sp_FiltrarEmpleados", CommandType.StoredProcedure, EmpleadoMapper.Map, parametros);
        }

        public Task<List<Empleado>> EmpleadosSinAlmorzarAsync(int idServicio)
        {
            var parametros = new[]
            {
                new SqlParameter("@IdServicio", idServicio)
            };

            return _accesoDatos.ListarAsync("sp_EmpleadosSinAlmorzar", CommandType.StoredProcedure, MapEmpleadoSinAlmorzar, parametros);
        }

        public Task<List<Empleado>> FiltrarSinAlmorzarAsync(int idServicio, int? idEmpresa, string? nombre)
        {
            var parametros = new[]
            {
                new SqlParameter("@IdServicio", idServicio),
                new SqlParameter("@IdEmpresa", (object?)idEmpresa ?? DBNull.Value),
                new SqlParameter("@Nombre", (object?)nombre ?? DBNull.Value)
            };

            return _accesoDatos.ListarAsync("sp_FiltrarEmpleadosSinAlmorzar", CommandType.StoredProcedure, MapEmpleadoSinAlmorzar, parametros);
        }
        #endregion

        #region Mappers Privados
        private static Empleado MapEmpleadoSinEstado(SqlDataReader reader)
        {
            return new Empleado
            {
                IdEmpleado  = DbMapper.GetInt32(reader, nameof(Empleado.IdEmpleado)),
                Nombre      = DbMapper.GetString(reader, nameof(Empleado.Nombre)),
                Apellido    = DbMapper.GetString(reader, nameof(Empleado.Apellido)),
                IdCredencial = DbMapper.GetString(reader, nameof(Empleado.IdCredencial)),
                IdEmpresa   = DbMapper.GetInt32(reader, nameof(Empleado.IdEmpresa)),
                Estado      = true,
                // El SP devuelve emp.Nombre as Empresa — lo cargamos en la navegación
                Empresa = new Empresa
                {
                    IdEmpresa = DbMapper.GetInt32(reader, nameof(Empleado.IdEmpresa)),
                    Nombre    = DbMapper.GetString(reader, "Empresa")
                }
            };
        }

        private static Empleado MapEmpleadoDetalle(SqlDataReader reader)
        {
            return new Empleado
            {
                IdEmpleado = DbMapper.GetInt32(reader, nameof(Empleado.IdEmpleado)),
                Nombre = DbMapper.GetString(reader, nameof(Empleado.Nombre)),
                Apellido = DbMapper.GetString(reader, nameof(Empleado.Apellido)),
                IdCredencial = DbMapper.GetString(reader, nameof(Empleado.IdCredencial)),
                IdEmpresa = DbMapper.GetInt32(reader, nameof(Empleado.IdEmpresa)),
                Estado = DbMapper.GetBoolean(reader, nameof(Empleado.Estado))
            };
        }

        private static Empleado MapEmpleadoSinAlmorzar(SqlDataReader reader)
        {
            return new Empleado
            {
                IdEmpleado = DbMapper.GetInt32(reader, nameof(Empleado.IdEmpleado)),
                Nombre = DbMapper.GetString(reader, nameof(Empleado.Nombre)),
                Apellido = DbMapper.GetString(reader, nameof(Empleado.Apellido)),
                IdCredencial = DbMapper.GetString(reader, nameof(Empleado.IdCredencial)),
                IdEmpresa = DbMapper.GetInt32(reader, nameof(Empleado.IdEmpresa)),
                Estado = true,
                Empresa = new Empresa
                {
                    IdEmpresa = DbMapper.GetInt32(reader, nameof(Empleado.IdEmpresa)),
                    Nombre = DbMapper.GetString(reader, "Empresa")
                }
            };
        }
        #endregion
    }
}