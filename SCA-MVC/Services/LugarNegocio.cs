using Microsoft.Data.SqlClient;
using SCA_MVC.Data;
using SCA_MVC.Data.Mappers;
using SCA_MVC.Models;
using System.Data;

namespace SCA_MVC.Services
{
    public class LugarNegocio : ILugarNegocio
    {
        private readonly AccesoDatos _accesoDatos;

        public LugarNegocio(AccesoDatos accesoDatos)
        {
            _accesoDatos = accesoDatos;
        }

        public Task<List<Lugar>> ListarAsync()
        {
            return _accesoDatos.ListarAsync("sp_ListarLugares", CommandType.StoredProcedure, MapLugarBasico);
        }

        public Task<Lugar?> BuscarPorNombreAsync(string nombre)
        {
            var parametros = new[]
            {
                new SqlParameter("@Nombre", nombre)
            };

            return _accesoDatos.ObtenerPrimeroAsync("sp_ObtenerLugarPorNombre", CommandType.StoredProcedure, LugarMapper.Map, parametros);
        }

        private static Lugar MapLugarBasico(SqlDataReader reader)
        {
            return new Lugar
            {
                IdLugar = DbMapper.GetInt32(reader, nameof(Lugar.IdLugar)),
                Nombre = DbMapper.GetString(reader, nameof(Lugar.Nombre)),
                Estado = true
            };
        }
    }
}