namespace LoanApi.Models;

public class Loan
{
    public string LoanNumber { get; set; } = string.Empty;
    public string ClientName { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public DateTime RequestDate { get; set; }
    public LoanStatus Status { get; set; }
    public ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();
}
