using Microsoft.Data.Sqlite;

namespace LoanApi.Data;

public sealed class MasterConnectionKeeper : IAsyncDisposable
{
    public SqliteConnection Connection { get; }

    public MasterConnectionKeeper(DatabaseConfig config)
    {
        Connection = new SqliteConnection(config.ConnectionString);
        Connection.Open();
    }

    public ValueTask DisposeAsync() => Connection.DisposeAsync();
}
