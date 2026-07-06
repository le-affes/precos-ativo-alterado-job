using Npgsql;

namespace Ativo.Alterado.Infrastrucuture.WebApplicationBuilder;

public static class ConnectionString
{
    public static string MontarConnectionStringPostgres(
        string dbUrl,
        string username,
        string password)
    {
        if (string.IsNullOrWhiteSpace(dbUrl))
        {
            throw new ArgumentException("DB_URL não pode ser vazio.", nameof(dbUrl));
        }

        if (dbUrl.Contains("Host=", StringComparison.OrdinalIgnoreCase))
        {
            var builder = new NpgsqlConnectionStringBuilder(dbUrl)
            {
                Username = username,
                Password = password
            };

            return builder.ConnectionString;
        }

        if (dbUrl.StartsWith("jdbc:postgresql://", StringComparison.OrdinalIgnoreCase))
        {
            dbUrl = dbUrl.Replace(
                "jdbc:postgresql://",
                "postgresql://",
                StringComparison.OrdinalIgnoreCase);
        }

        if (!dbUrl.StartsWith("postgresql://", StringComparison.OrdinalIgnoreCase))
        {
            dbUrl = $"postgresql://{dbUrl}";
        }

        var uri = new Uri(dbUrl);

        var builderFromUri = new NpgsqlConnectionStringBuilder
        {
            Host = uri.Host,
            Port = uri.Port > 0 ? uri.Port : 5432,
            Database = uri.AbsolutePath.TrimStart('/'),
            Username = username,
            Password = password
        };

        return builderFromUri.ConnectionString;
    }
}