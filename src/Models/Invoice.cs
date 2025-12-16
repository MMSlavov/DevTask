namespace LoanApi.Models;

public class Invoice
{
    public string InvoiceNumber { get; set; } = string.Empty;
    public decimal Amount { get; set; }
}
