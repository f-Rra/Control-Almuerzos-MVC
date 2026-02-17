using Microsoft.Data.SqlClient;
using SCA_MVC.Data;
using SCA_MVC.Data.Mappers;
using SCA_MVC.Models;
using System.Data;

namespace SCA_MVC.Services
{
    public class EmpresaNegocio : IEmpresaNegocio
    {
        private readonly AccesoDatos _accesoDatos;

        public EmpresaNegocio(AccesoDatos accesoDatos)
        {
            _accesoDatos = accesoDatos;
        }

        public Task<List<Empresa>> ListarAsync()
        {
            return _accesoDatos.ListarAsync("sp_ListarEmpresas", CommandType.StoredProcedure, MapEmpresaBasica);
        }

        public Task<List<Empresa>> ListarConEmpleadosAsync()
        {
            return _accesoDatos.ListarAsync("sp_ListarEmpresasConEmpleados", CommandType.StoredProcedure, EmpresaMapper.Map);
        }

        public Task<Empresa?> BuscarPorIdAsync(int idEmpresa)
        {
            var parametros = new[]
            {
                new SqlParameter("@IdEmpresa", idEmpresa)
            };

            return _accesoDatos.ObtenerPrimeroAsync("sp_BuscarEmpresaPorId", CommandType.StoredProcedure, EmpresaMapper.Map, parametros);
        }

        public async Task<int> AgregarAsync(Empresa empresa)
        {
            var parametros = new[]
            {
                new SqlParameter("@Nombre", empresa.Nombre),
                new SqlParameter("@Estado", empresa.Estado)
            };

            var id = await _accesoDatos.EscalarAsync("sp_AgregarEmpresa", CommandType.StoredProcedure, parametros);
            return Convert.ToInt32(id);
        }

        public Task ModificarAsync(Empresa empresa)
        {
            var parametros = new[]
            {
                new SqlParameter("@IdEmpresa", empresa.IdEmpresa),
                new SqlParameter("@Nombre", empresa.Nombre),
                new SqlParameter("@Estado", empresa.Estado)
            };

            return _accesoDatos.EjecutarAsync("sp_ModificarEmpresa", CommandType.StoredProcedure, parametros);
        }

        public Task EliminarAsync(int idEmpresa)
        {
            var parametros = new[]
            {
                new SqlParameter("@IdEmpresa", idEmpresa)
            };

            return _accesoDatos.EjecutarAsync("sp_DesactivarEmpresa", CommandType.StoredProcedure, parametros);
        }

        public Task<List<Empresa>> FiltrarAsync(string? filtro)
        {
            var parametros = new[]
            {
                new SqlParameter("@Filtro", (object?)filtro ?? DBNull.Value)
            };

            return _accesoDatos.ListarAsync("sp_FiltrarEmpresas", CommandType.StoredProcedure, EmpresaMapper.Map, parametros);
        }

        private static Empresa MapEmpresaBasica(SqlDataReader reader)
        {
            return new Empresa
            {
                IdEmpresa = DbMapper.GetInt32(reader, nameof(Empresa.IdEmpresa)),
                Nombre = DbMapper.GetString(reader, nameof(Empresa.Nombre)),
                Estado = true
            };
        }
    }
}