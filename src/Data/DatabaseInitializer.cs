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

        CREATE TABLE IF NOT EXISTS Invoices (
            InvoiceNumber TEXT PRIMARY KEY,
            LoanNumber TEXT NOT NULL,
            Amount REAL NOT NULL,
            FOREIGN KEY (LoanNumber) REFERENCES Loans(LoanNumber)
        );
        """;

        await connection.ExecuteAsync(createTableSql);

        const string countSql = "SELECT COUNT(1) FROM Loans;";
        var existing = await connection.ExecuteScalarAsync<int>(countSql);

        if (existing > 0)
        {
            return;
        }

        const string insertLoanSql = """
        INSERT INTO Loans (LoanNumber, ClientName, Amount, RequestDate, Status)
        VALUES (@LoanNumber, @ClientName, @Amount, @RequestDate, @Status);
        """;

        const string insertInvoiceSql = """
        INSERT INTO Invoices (InvoiceNumber, LoanNumber, Amount)
        VALUES (@InvoiceNumber, @LoanNumber, @Amount);
        """;

        var seedLoans = new List<Loan>
        {
            new()
            {
                LoanNumber = "LN-1001",
                ClientName = "Alice Johnson",
                Amount = 25_000,
                RequestDate = DateTime.UtcNow.AddDays(-10),
                Status = LoanStatus.Paid,
                Invoices = new List<Invoice>()
                {
                    new Invoice { InvoiceNumber = "INV-1001-1", Amount = 10_000 },
                    new Invoice { InvoiceNumber = "INV-1001-2", Amount = 15_000 }
                }
            },
            new()
            {
                LoanNumber = "LN-1002",
                ClientName = "Brandon Smith",
                Amount = 50_000.50m,
                RequestDate = DateTime.UtcNow.AddDays(-5),
                Status = LoanStatus.AwaitingPayment,
                Invoices = new List<Invoice>()
                {
                    new Invoice { InvoiceNumber = "INV-1002-1", Amount = 25_000.25m }
                }
            },
            new()
            {
                LoanNumber = "LN-1003",
                ClientName = "Casey Diaz",
                Amount = 75_000,
                RequestDate = DateTime.UtcNow.AddDays(-2),
                Status = LoanStatus.Created,
                Invoices = new List<Invoice>()
            }
        };

        foreach (var loan in seedLoans)
        {
            await connection.ExecuteAsync(insertLoanSql, new
            {
                loan.LoanNumber,
                loan.ClientName,
                loan.Amount,
                RequestDate = loan.RequestDate.ToString("O"),
                Status = loan.Status.ToString()
            });

            foreach (var inv in loan.Invoices)
            {
                await connection.ExecuteAsync(insertInvoiceSql, new
                {
                    inv.InvoiceNumber,
                    loan.LoanNumber,
                    inv.Amount
                });
            }
        }
    }
}
