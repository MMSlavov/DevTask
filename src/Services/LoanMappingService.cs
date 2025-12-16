using LoanApi.Models;
using LoanApi.Models.Responses;

namespace LoanApi.Services;

public class LoanMappingService
{
    public IEnumerable<LoanResponse> MapLoansToResponses(IEnumerable<Loan> loans)
    {
        return loans.Select(MapLoanToResponse);
    }

    public LoanStatsResponse MapStatsToResponse(decimal sumPaid, decimal sumAwaiting)
    {
        var total = sumPaid + sumAwaiting;
        var pctPaid = total == 0 ? 0 : sumPaid / total * 100m;
        var pctAwaiting = total == 0 ? 0 : sumAwaiting / total * 100m;

        return new LoanStatsResponse
        {
            SumPaid = sumPaid,
            SumAwaitingPayment = sumAwaiting,
            PercentagePaid = decimal.Round(pctPaid, 2),
            PercentageAwaitingPayment = decimal.Round(pctAwaiting, 2)
        };
    }

    private LoanResponse MapLoanToResponse(Loan loan)
    {
        return new LoanResponse
        {
            LoanNumber = loan.LoanNumber,
            ClientName = loan.ClientName,
            Amount = loan.Amount,
            RequestDate = loan.RequestDate,
            Status = loan.Status.ToString(),
            Invoices = loan.Invoices.Select(MapInvoiceToResponse)
        };
    }

    private InvoiceResponse MapInvoiceToResponse(Invoice invoice)
    {
        return new InvoiceResponse
        {
            InvoiceNumber = invoice.InvoiceNumber,
            Amount = invoice.Amount
        };
    }
}
