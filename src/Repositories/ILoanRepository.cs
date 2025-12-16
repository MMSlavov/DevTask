using LoanApi.Models;

namespace LoanApi.Repositories;

public interface ILoanRepository
{
    Task<IEnumerable<Loan>> GetAllAsync();
}
