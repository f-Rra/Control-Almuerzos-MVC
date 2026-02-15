using Microsoft.Data.SqlClient;
using SCA_MVC.Models;

namespace SCA_MVC.Data.Mappers
{
    public static class ServicioMapper
    {
        public static Servicio Map(SqlDataReader reader)
        {
            return new Servicio
            {
                IdServicio = DbMapper.GetInt32(reader, nameof(Servicio.IdServicio)),
                IdLugar = DbMapper.GetInt32(reader, nameof(Servicio.IdLugar)),
                Fecha = DbMapper.GetDateTime(reader, nameof(Servicio.Fecha)),
                Proyeccion = DbMapper.GetNullableInt32(reader, nameof(Servicio.Proyeccion)),
                DuracionMinutos = DbMapper.GetNullableInt32(reader, nameof(Servicio.DuracionMinutos)),
                TotalComensales = DbMapper.GetInt32(reader, nameof(Servicio.TotalComensales)),
                TotalInvitados = DbMapper.GetInt32(reader, nameof(Servicio.TotalInvitados))
            };
        }
    }
}