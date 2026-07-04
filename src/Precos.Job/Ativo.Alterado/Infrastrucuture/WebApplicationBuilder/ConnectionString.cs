using Npgsql;

namespace Ativo.Alterado.Infrastrucuture.WebApplicationBuilder;

public static class ConnectionString
{
    public static string MontarConnectionStringPostgres(string dbUrl, string username, string password)
    {
        if (dbUrl.Contains("Host=", StringComparison.OrdinalIgnoreCase))
        {
            return $"{dbUrl};Username={username};Password={password}";
        }

        if (dbUrl.StartsWith("jdbc:postgresql://", StringComparison.OrdinalIgnoreCase))
        {
            dbUrl = dbUrl.Replace("jdbc:postgresql://", "postgresql://", StringComparison.OrdinalIgnoreCase);
        }

        var uri = new Uri(dbUrl);

        var builder = new NpgsqlConnectionStringBuilder
        {
            Host = uri.Host,
            Port = uri.Port > 0 ? uri.Port : 5432,
            Database = uri.AbsolutePath.TrimStart('/'),
            Username = username,
            Password = password
        };

        return builder.ConnectionString;
    }

}