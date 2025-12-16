namespace LoanApi.Models.Responses;

public class LoanStatsResponse
{
    public decimal SumPaid { get; set; }
    public decimal SumAwaitingPayment { get; set; }
    public decimal PercentagePaid { get; set; }
    public decimal PercentageAwaitingPayment { get; set; }
}
