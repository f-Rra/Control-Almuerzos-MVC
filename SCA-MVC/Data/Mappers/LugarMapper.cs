using Microsoft.Data.SqlClient;
using SCA_MVC.Models;

namespace SCA_MVC.Data.Mappers
{
    public static class LugarMapper
    {
        public static Lugar Map(SqlDataReader reader)
        {
            return new Lugar
            {
                IdLugar = DbMapper.GetInt32(reader, nameof(Lugar.IdLugar)),
                Nombre = DbMapper.GetString(reader, nameof(Lugar.Nombre)),
                Estado = DbMapper.GetBoolean(reader, nameof(Lugar.Estado))
            };
        }
    }
}