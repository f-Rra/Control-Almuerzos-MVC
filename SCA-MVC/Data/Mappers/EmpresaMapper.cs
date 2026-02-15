using Microsoft.Data.SqlClient;
using SCA_MVC.Models;

namespace SCA_MVC.Data.Mappers
{
    public static class EmpresaMapper
    {
        public static Empresa Map(SqlDataReader reader)
        {
            return new Empresa
            {
                IdEmpresa = DbMapper.GetInt32(reader, nameof(Empresa.IdEmpresa)),
                Nombre = DbMapper.GetString(reader, nameof(Empresa.Nombre)),
                Estado = DbMapper.GetBoolean(reader, nameof(Empresa.Estado))
            };
        }
    }
}