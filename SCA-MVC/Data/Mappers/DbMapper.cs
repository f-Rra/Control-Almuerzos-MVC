using Microsoft.Data.SqlClient;

namespace SCA_MVC.Data.Mappers
{
    public static class DbMapper
    {
        public static int GetInt32(SqlDataReader reader, string column)
        {
            var ordinal = reader.GetOrdinal(column);
            return reader.IsDBNull(ordinal) ? default : reader.GetInt32(ordinal);
        }

        public static int? GetNullableInt32(SqlDataReader reader, string column)
        {
            var ordinal = reader.GetOrdinal(column);
            return reader.IsDBNull(ordinal) ? null : reader.GetInt32(ordinal);
        }

        public static string GetString(SqlDataReader reader, string column)
        {
            var ordinal = reader.GetOrdinal(column);
            return reader.IsDBNull(ordinal) ? string.Empty : reader.GetString(ordinal);
        }

        public static bool GetBoolean(SqlDataReader reader, string column)
        {
            var ordinal = reader.GetOrdinal(column);
            return !reader.IsDBNull(ordinal) && reader.GetBoolean(ordinal);
        }

        public static DateTime GetDateTime(SqlDataReader reader, string column)
        {
            var ordinal = reader.GetOrdinal(column);
            return reader.IsDBNull(ordinal) ? default : reader.GetDateTime(ordinal);
        }

        public static TimeSpan GetTimeSpan(SqlDataReader reader, string column)
        {
            var ordinal = reader.GetOrdinal(column);
            return reader.IsDBNull(ordinal) ? default : reader.GetTimeSpan(ordinal);
        }
    }
}