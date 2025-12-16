using LoanApi.Models;

namespace LoanApi.Data;

public interface ILoanRepository
{
    Task<IEnumerable<Loan>> GetAllAsync();
    Task<Loan?> GetByLoanNumberAsync(string loanNumber);
    Task<Loan> CreateAsync(Loan loan);
    Task<bool> UpdateAsync(Loan loan);
    Task<bool> DeleteAsync(string loanNumber);
}
