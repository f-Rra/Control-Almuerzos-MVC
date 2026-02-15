using Microsoft.Data.SqlClient;
using SCA_MVC.Models;

namespace SCA_MVC.Data.Mappers
{
    public static class RegistroMapper
    {
        public static Registro Map(SqlDataReader reader)
        {
            return new Registro
            {
                IdRegistro = DbMapper.GetInt32(reader, nameof(Registro.IdRegistro)),
                IdEmpleado = DbMapper.GetNullableInt32(reader, nameof(Registro.IdEmpleado)),
                IdEmpresa = DbMapper.GetInt32(reader, nameof(Registro.IdEmpresa)),
                IdServicio = DbMapper.GetInt32(reader, nameof(Registro.IdServicio)),
                IdLugar = DbMapper.GetInt32(reader, nameof(Registro.IdLugar)),
                Fecha = DbMapper.GetDateTime(reader, nameof(Registro.Fecha)),
                Hora = DbMapper.GetTimeSpan(reader, nameof(Registro.Hora))
            };
        }
    }
}