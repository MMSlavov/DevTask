using Microsoft.Data.Sqlite;
using Dapper;

namespace LoanApi.Data;

public class DatabaseInitializer
{
    public static SqliteConnection InitializeDatabase(string connectionString)
    {
        var connection = new SqliteConnection(connectionString);
        connection.Open();

        var createTableQuery = @"
            CREATE TABLE IF NOT EXISTS Loans (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                LoanNumber TEXT NOT NULL UNIQUE,
                ClientName TEXT NOT NULL,
                Amount DECIMAL(18, 2) NOT NULL,
                RequestDate TEXT NOT NULL,
                Status TEXT NOT NULL
            )";

        connection.Execute(createTableQuery);
        
        return connection;
    }
}
