using Dapper;
using LoanApi.Models;

namespace LoanApi.Data;

public class DatabaseInitializer
{
    private readonly MasterConnectionKeeper _masterConnectionKeeper;

    public DatabaseInitializer(MasterConnectionKeeper masterConnectionKeeper)
    {
        _masterConnectionKeeper = masterConnectionKeeper;
    }

    public async Task InitializeAsync()
    {
        var connection = _masterConnectionKeeper.Connection;

        const string createTableSql = """
        CREATE TABLE IF NOT EXISTS Loans (
            LoanNumber TEXT PRIMARY KEY,
            ClientName TEXT NOT NULL,
            Amount REAL NOT NULL,
            RequestDate TEXT NOT NULL,
            Status TEXT NOT NULL
        );
        """;

        await connection.ExecuteAsync(createTableSql);

        const string countSql = "SELECT COUNT(1) FROM Loans;";
        var existing = await connection.ExecuteScalarAsync<int>(countSql);

        if (existing > 0)
        {
            return;
        }

        const string insertSql = """
        INSERT INTO Loans (LoanNumber, ClientName, Amount, RequestDate, Status)
        VALUES (@LoanNumber, @ClientName, @Amount, @RequestDate, @Status);
        """;

        var seedLoans = new List<Loan>
        {
            new()
            {
                LoanNumber = "LN-1001",
                ClientName = "Alice Johnson",
                Amount = 25_000,
                RequestDate = DateTime.UtcNow.AddDays(-10),
                Status = LoanStatus.Paid
            },
            new()
            {
                LoanNumber = "LN-1002",
                ClientName = "Brandon Smith",
                Amount = 50_000,
                RequestDate = DateTime.UtcNow.AddDays(-5),
                Status = LoanStatus.AwaitingPayment
            },
            new()
            {
                LoanNumber = "LN-1003",
                ClientName = "Casey Diaz",
                Amount = 75_000,
                RequestDate = DateTime.UtcNow.AddDays(-2),
                Status = LoanStatus.Created
            }
        };

        foreach (var loan in seedLoans)
        {
            await connection.ExecuteAsync(insertSql, new
            {
                loan.LoanNumber,
                loan.ClientName,
                loan.Amount,
                RequestDate = loan.RequestDate.ToString("O"),
                Status = loan.Status.ToString()
            });
        }
    }
}
