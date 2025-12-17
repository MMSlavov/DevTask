namespace LoanApi.Constants;

public static class SqlQueries
{
    public const string GetAllLoans = """
        SELECT LoanNumber, ClientName, Amount, RequestDate, Status
        FROM Loans
        ORDER BY RequestDate DESC;
        """;

    public const string GetInvoicesByLoanNumbers = """
        SELECT LoanNumber, InvoiceNumber, Amount FROM Invoices WHERE LoanNumber IN @LoanNumbers;
        """;

    public const string GetPaidAwaitingSums = """
        SELECT Status, SUM(Amount) AS TotalAmount
        FROM Loans
        WHERE Status IN ('Paid', 'AwaitingPayment')
        GROUP BY Status;
        """;
}
