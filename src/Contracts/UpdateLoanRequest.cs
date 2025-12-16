namespace LoanApi.Contracts;

public class UpdateLoanRequest
{
    public string? ClientName { get; init; }
    public decimal? Amount { get; init; }
    public DateTime? RequestDate { get; init; }
    public string? Status { get; init; }
}
