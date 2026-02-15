using Microsoft.Data.SqlClient;
using SCA_MVC.Models;

namespace SCA_MVC.Data.Mappers
{
    public static class EmpleadoMapper
    {
        public static Empleado Map(SqlDataReader reader)
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
    }
}