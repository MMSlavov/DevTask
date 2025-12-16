namespace LoanApi.Models.Responses;

public class InvoiceResponse
{
    public string InvoiceNumber { get; set; } = string.Empty;
    public decimal Amount { get; set; }
}
