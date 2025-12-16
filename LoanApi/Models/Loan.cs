namespace LoanApi.Models;

public class Loan
{
    public int Id { get; set; }
    public string LoanNumber { get; set; } = string.Empty;
    public string ClientName { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public DateTime RequestDate { get; set; }
    public string Status { get; set; } = string.Empty;
}
