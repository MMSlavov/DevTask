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

        const string invoiceSql = """
        SELECT LoanNumber, InvoiceNumber, Amount FROM Invoices WHERE LoanNumber IN @LoanNumbers;
        """;

        using var connection = await _connectionFactory.CreateConnectionAsync();
        var rows = await connection.QueryAsync<LoanRow>(sql);
        var loans = rows.Select(MapLoan).ToList();

        if (loans.Any())
        {
            var loanNumbers = loans.Select(l => l.LoanNumber).ToArray();
            var invoiceRows = await connection.QueryAsync<InvoiceRow>(invoiceSql, new { LoanNumbers = loanNumbers });
            var grouped = invoiceRows.GroupBy(i => i.LoanNumber)
                                     .ToDictionary(g => g.Key, g => g.Select(ir => new Invoice
                                     {
                                         InvoiceNumber = ir.InvoiceNumber,
                                         Amount = (decimal)ir.Amount
                                     }).ToList());

            foreach (var loan in loans)
            {
                if (grouped.TryGetValue(loan.LoanNumber, out var invoices))
                {
                    loan.Invoices = invoices;
                }
            }
        }

        return loans;
    }

    public async Task<(decimal SumPaid, decimal SumAwaitingPayment)> GetPaidAwaitingSumsAsync()
    {
        const string sql = """
        SELECT Status, SUM(Amount) AS TotalAmount
        FROM Loans
        WHERE Status IN ('Paid', 'AwaitingPayment')
        GROUP BY Status;
        """;

        using var connection = await _connectionFactory.CreateConnectionAsync();
        var rows = await connection.QueryAsync<SumRow>(sql);

        var sumPaid = (decimal?)rows.FirstOrDefault(x => x.Status == LoanStatus.Paid.ToString())?.TotalAmount;
        var sumAwaiting = (decimal?)rows.FirstOrDefault(x => x.Status == LoanStatus.AwaitingPayment.ToString())?.TotalAmount;

        return (sumPaid ?? 0m, sumAwaiting ?? 0m);
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
    private sealed record InvoiceRow(string LoanNumber, string InvoiceNumber, double Amount);
    private sealed record SumRow(string Status, double TotalAmount);
}
