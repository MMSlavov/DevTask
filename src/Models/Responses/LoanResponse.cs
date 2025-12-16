namespace LoanApi.Models.Responses;

public class LoanResponse
{
    public string LoanNumber { get; set; } = string.Empty;
    public string ClientName { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public DateTime RequestDate { get; set; }
    public string Status { get; set; } = string.Empty;
    public IEnumerable<InvoiceResponse> Invoices { get; set; } = new List<InvoiceResponse>();
}
