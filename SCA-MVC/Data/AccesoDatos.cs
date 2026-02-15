using Microsoft.Data.SqlClient;
using System.Data;

namespace SCA_MVC.Data
{
    public class AccesoDatos
    {
        private readonly string _connectionString;

        public AccesoDatos(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("No se encontró la cadena de conexión 'DefaultConnection'.");
        }

        public async Task<int> EjecutarAsync(string sql, CommandType commandType, IEnumerable<SqlParameter>? parametros = null)
        {
            try
            {
                await using var connection = new SqlConnection(_connectionString);
                await using var command = CrearComando(connection, sql, commandType, parametros);

                await connection.OpenAsync();
                return await command.ExecuteNonQueryAsync();
            }
            catch (SqlException ex)
            {
                throw NegocioException.DesdeSqlException(ex);
            }
        }

        public async Task<T?> ObtenerPrimeroAsync<T>(
            string sql,
            CommandType commandType,
            Func<SqlDataReader, T> mapper,
            IEnumerable<SqlParameter>? parametros = null)
        {
            try
            {
                await using var connection = new SqlConnection(_connectionString);
                await using var command = CrearComando(connection, sql, commandType, parametros);

                await connection.OpenAsync();
                await using var reader = await command.ExecuteReaderAsync();

                if (await reader.ReadAsync())
                {
                    return mapper(reader);
                }

                return default;
            }
            catch (SqlException ex)
            {
                throw NegocioException.DesdeSqlException(ex);
            }
        }

        public async Task<List<T>> ListarAsync<T>(
            string sql,
            CommandType commandType,
            Func<SqlDataReader, T> mapper,
            IEnumerable<SqlParameter>? parametros = null)
        {
            var resultados = new List<T>();

            try
            {
                await using var connection = new SqlConnection(_connectionString);
                await using var command = CrearComando(connection, sql, commandType, parametros);

                await connection.OpenAsync();
                await using var reader = await command.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    resultados.Add(mapper(reader));
                }

                return resultados;
            }
            catch (SqlException ex)
            {
                throw NegocioException.DesdeSqlException(ex);
            }
        }

        public async Task<object?> EscalarAsync(string sql, CommandType commandType, IEnumerable<SqlParameter>? parametros = null)
        {
            try
            {
                await using var connection = new SqlConnection(_connectionString);
                await using var command = CrearComando(connection, sql, commandType, parametros);

                await connection.OpenAsync();
                return await command.ExecuteScalarAsync();
            }
            catch (SqlException ex)
            {
                throw NegocioException.DesdeSqlException(ex);
            }
        }

        private static SqlCommand CrearComando(
            SqlConnection connection,
            string sql,
            CommandType commandType,
            IEnumerable<SqlParameter>? parametros)
        {
            var command = new SqlCommand(sql, connection)
            {
                CommandType = commandType,
                CommandTimeout = 30
            };

            if (parametros != null)
            {
                command.Parameters.AddRange(parametros.ToArray());
            }

            return command;
        }
    }
}