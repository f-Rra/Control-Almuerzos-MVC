using Microsoft.Data.SqlClient;

namespace SCA_MVC.Data
{
    public class NegocioException : Exception
    {
        public NegocioException(string mensaje)
            : base(mensaje)
        {
        }

        public NegocioException(string mensaje, Exception innerException)
            : base(mensaje, innerException)
        {
        }

        public static NegocioException DesdeSqlException(SqlException ex)
        {
            var mensaje = ex.Number switch
            {
                2627 or 2601 => "Ya existe un registro con esos datos.",
                547 => "No se puede realizar la operación porque existen datos relacionados.",
                515 => "Faltan datos obligatorios para completar la operación.",
                245 => "Uno de los valores ingresados no tiene un formato válido.",
                1205 => "La operación no pudo completarse por concurrencia. Intentá nuevamente.",
                _ => "Ocurrió un error de base de datos. Intentá nuevamente."
            };

            return new NegocioException(mensaje, ex);
        }
    }
}