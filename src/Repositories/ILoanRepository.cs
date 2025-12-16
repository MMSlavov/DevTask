using LoanApi.Models;

namespace LoanApi.Repositories;

public interface ILoanRepository
{
    Task<IEnumerable<Loan>> GetAllAsync();
    Task<(decimal SumPaid, decimal SumAwaitingPayment)> GetPaidAwaitingSumsAsync();
}
