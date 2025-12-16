using System.Globalization;
using Dapper;
using LoanApi.Data;
using LoanApi.Models;

namespace LoanApi.Repositories;

public class LoanRepository : ILoanRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public LoanRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<IEnumerable<Loan>> GetAllAsync()
    {
        const string sql = """
        SELECT LoanNumber, ClientName, Amount, RequestDate, Status
        FROM Loans
        ORDER BY RequestDate DESC;
        """;

        using var connection = await _connectionFactory.CreateConnectionAsync();
        var rows = await connection.QueryAsync<LoanRow>(sql);
        return rows.Select(MapLoan);
    }

    private static Loan MapLoan(LoanRow row)
    {
        var status = Enum.TryParse<LoanStatus>(row.Status, true, out var parsedStatus)
            ? parsedStatus
            : LoanStatus.AwaitingPayment;

        var requestDate = DateTime.TryParse(row.RequestDate, null, DateTimeStyles.RoundtripKind, out var parsedDate)
            ? parsedDate
            : DateTime.SpecifyKind(DateTime.Parse(row.RequestDate), DateTimeKind.Utc);

        return new Loan
        {
            LoanNumber = row.LoanNumber,
            ClientName = row.ClientName,
            Amount = (decimal)row.Amount,
            RequestDate = requestDate,
            Status = status
        };
    }

    private sealed record LoanRow(string LoanNumber, string ClientName, double Amount, string RequestDate, string Status);
}
