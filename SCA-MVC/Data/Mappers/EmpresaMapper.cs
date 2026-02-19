using Microsoft.Data.SqlClient;
using SCA_MVC.Models;

namespace SCA_MVC.Data.Mappers
{
    public static class EmpresaMapper
    {
        public static Empresa Map(SqlDataReader reader)
        {
            var empresa = new Empresa
            {
                IdEmpresa = DbMapper.GetInt32(reader, nameof(Empresa.IdEmpresa)),
                Nombre = DbMapper.GetString(reader, nameof(Empresa.Nombre)),
                Estado = DbMapper.GetBoolean(reader, nameof(Empresa.Estado))
            };

            // Intentar leer CantidadEmpleados solo si existe la columna
            try
            {
                empresa.CantidadEmpleados = DbMapper.GetInt32(reader, "CantidadEmpleados");
            }
            catch (IndexOutOfRangeException)
            {
                // La columna no existe, dejar en 0
                empresa.CantidadEmpleados = 0;
            }

            return empresa;
        }
    }
}