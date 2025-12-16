using Microsoft.Data.Sqlite;
using Dapper;
using LoanApi.Models;

namespace LoanApi.Data;

public class LoanRepository : ILoanRepository
{
    private readonly SqliteConnection _connection;

    public LoanRepository(SqliteConnection connection)
    {
        _connection = connection;
    }

    public async Task<IEnumerable<Loan>> GetAllAsync()
    {
        var query = "SELECT * FROM Loans";
        return await _connection.QueryAsync<Loan>(query);
    }

    public async Task<Loan?> GetByLoanNumberAsync(string loanNumber)
    {
        var query = "SELECT * FROM Loans WHERE LoanNumber = @LoanNumber";
        return await _connection.QueryFirstOrDefaultAsync<Loan>(query, new { LoanNumber = loanNumber });
    }

    public async Task<Loan> CreateAsync(Loan loan)
    {
        var query = @"
            INSERT INTO Loans (LoanNumber, ClientName, Amount, RequestDate, Status)
            VALUES (@LoanNumber, @ClientName, @Amount, @RequestDate, @Status);
            SELECT * FROM Loans WHERE Id = last_insert_rowid();";
        
        return await _connection.QuerySingleAsync<Loan>(query, loan);
    }

    public async Task<bool> UpdateAsync(Loan loan)
    {
        var query = @"
            UPDATE Loans
            SET ClientName = @ClientName,
                Amount = @Amount,
                RequestDate = @RequestDate,
                Status = @Status
            WHERE LoanNumber = @LoanNumber";
        
        var affectedRows = await _connection.ExecuteAsync(query, loan);
        return affectedRows > 0;
    }

    public async Task<bool> DeleteAsync(string loanNumber)
    {
        var query = "DELETE FROM Loans WHERE LoanNumber = @LoanNumber";
        var affectedRows = await _connection.ExecuteAsync(query, new { LoanNumber = loanNumber });
        return affectedRows > 0;
    }
}
