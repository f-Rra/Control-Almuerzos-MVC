using Microsoft.Data.SqlClient;
using SCA_MVC.ViewModels;
using System;

namespace SCA_MVC.Data.Mappers
{
    public static class ReporteMapper
    {
        public static ServicioReporte MapServicio(SqlDataReader reader)
        {
            return new ServicioReporte
            {
                IdServicio      = DbMapper.GetInt32(reader, "IdServicio"),
                Fecha           = DbMapper.GetDateTime(reader, "Fecha"),
                Lugar           = DbMapper.GetString(reader, "NombreLugar"),
                Proyeccion      = DbMapper.GetInt32(reader, "Proyeccion"),
                DuracionMinutos = reader.IsDBNull(reader.GetOrdinal("DuracionMinutos")) ? null : reader.GetInt32(reader.GetOrdinal("DuracionMinutos")),
                TotalComensales = DbMapper.GetInt32(reader, "TotalComensales"),
                TotalInvitados  = DbMapper.GetInt32(reader, "TotalInvitados")
            };
        }

        public static AsistenciaPorEmpresa MapAsistenciaEmpresa(SqlDataReader reader)
        {
            return new AsistenciaPorEmpresa
            {
                Empresa = DbMapper.GetString(reader, "Empresa"),
                TotalAsistencias = DbMapper.GetInt32(reader, "TotalAsistencias")
            };
        }

        public static AsistenciaPorDia MapAsistenciaDia(SqlDataReader reader)
        {
            return new AsistenciaPorDia
            {
                Orden = DbMapper.GetInt32(reader, "Orden"),
                Dia = DbMapper.GetString(reader, "Dia"),
                Total = DbMapper.GetInt32(reader, "Total")
            };
        }

        public static CoberturaReporte MapCobertura(SqlDataReader reader)
        {
            return new CoberturaReporte
            {
                Fecha               = DbMapper.GetDateTime(reader, "Fecha"),
                Lugar               = DbMapper.GetString(reader, "Lugar"),
                Proyeccion          = DbMapper.GetInt32(reader, "Proyeccion"),
                Atendidos           = DbMapper.GetInt32(reader, "Atendidos"),
                CoberturaPorcentaje = reader.IsDBNull(reader.GetOrdinal("CoberturaPorcentaje")) ? null : reader.GetDecimal(reader.GetOrdinal("CoberturaPorcentaje")),
                Diferencia          = DbMapper.GetInt32(reader, "Diferencia")
            };
        }
    }
}
