namespace LoanApi.Contracts;

public record CreateLoanRequest
(
    string LoanNumber,
    string ClientName,
    decimal Amount,
    DateTime RequestDate,
    string Status
);
