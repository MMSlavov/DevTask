using System.Data;
using Microsoft.Data.Sqlite;

namespace LoanApi.Data;

public class SqliteConnectionFactory : IDbConnectionFactory
{
    private readonly DatabaseConfig _config;

    public SqliteConnectionFactory(DatabaseConfig config)
    {
        _config = config;
    }

    public async Task<IDbConnection> CreateConnectionAsync()
    {
        var connection = new SqliteConnection(_config.ConnectionString);
        await connection.OpenAsync();
        return connection;
    }
}
