using LoanApi.Models;

namespace LoanApi.Contracts;

public record LoanResponse(
    string LoanNumber,
    string ClientName,
    decimal Amount,
    DateTime RequestDate,
    string Status);

public static class LoanResponseMapper
{
    public static LoanResponse FromModel(Loan loan) => new(
        loan.LoanNumber,
        loan.ClientName,
        loan.Amount,
        loan.RequestDate,
        loan.Status.ToString());
}
