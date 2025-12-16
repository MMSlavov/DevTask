using Microsoft.AspNetCore.Mvc;
using LoanApi.Data;
using LoanApi.Models;

namespace LoanApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LoansController : ControllerBase
{
    private readonly ILoanRepository _loanRepository;
    private readonly ILogger<LoansController> _logger;

    public LoansController(ILoanRepository loanRepository, ILogger<LoansController> logger)
    {
        _loanRepository = loanRepository;
        _logger = logger;
    }

    /// <summary>
    /// Gets all loans
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Loan>>> GetAllLoans()
    {
        try
        {
            var loans = await _loanRepository.GetAllAsync();
            return Ok(loans);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all loans");
            return StatusCode(500, "An error occurred while retrieving loans");
        }
    }

    /// <summary>
    /// Gets a specific loan by loan number
    /// </summary>
    [HttpGet("{loanNumber}")]
    public async Task<ActionResult<Loan>> GetLoan(string loanNumber)
    {
        try
        {
            var loan = await _loanRepository.GetByLoanNumberAsync(loanNumber);
            if (loan == null)
            {
                return NotFound($"Loan with number {loanNumber} not found");
            }
            return Ok(loan);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting loan {LoanNumber}", loanNumber);
            return StatusCode(500, "An error occurred while retrieving the loan");
        }
    }

    /// <summary>
    /// Creates a new loan
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<Loan>> CreateLoan([FromBody] Loan loan)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(loan.LoanNumber))
            {
                return BadRequest("Loan number is required");
            }

            if (string.IsNullOrWhiteSpace(loan.ClientName))
            {
                return BadRequest("Client name is required");
            }

            if (loan.Amount <= 0)
            {
                return BadRequest("Amount must be greater than zero");
            }

            // Check if loan with the same number already exists
            var existingLoan = await _loanRepository.GetByLoanNumberAsync(loan.LoanNumber);
            if (existingLoan != null)
            {
                return Conflict($"Loan with number {loan.LoanNumber} already exists");
            }

            var createdLoan = await _loanRepository.CreateAsync(loan);
            return CreatedAtAction(nameof(GetLoan), new { loanNumber = createdLoan.LoanNumber }, createdLoan);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating loan");
            return StatusCode(500, "An error occurred while creating the loan");
        }
    }

    /// <summary>
    /// Updates an existing loan
    /// </summary>
    [HttpPut("{loanNumber}")]
    public async Task<ActionResult> UpdateLoan(string loanNumber, [FromBody] Loan loan)
    {
        try
        {
            if (loanNumber != loan.LoanNumber)
            {
                return BadRequest("Loan number in URL does not match loan number in body");
            }

            var existingLoan = await _loanRepository.GetByLoanNumberAsync(loanNumber);
            if (existingLoan == null)
            {
                return NotFound($"Loan with number {loanNumber} not found");
            }

            var updated = await _loanRepository.UpdateAsync(loan);
            if (updated)
            {
                return NoContent();
            }

            return StatusCode(500, "Failed to update the loan");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating loan {LoanNumber}", loanNumber);
            return StatusCode(500, "An error occurred while updating the loan");
        }
    }

    /// <summary>
    /// Deletes a loan
    /// </summary>
    [HttpDelete("{loanNumber}")]
    public async Task<ActionResult> DeleteLoan(string loanNumber)
    {
        try
        {
            var deleted = await _loanRepository.DeleteAsync(loanNumber);
            if (deleted)
            {
                return NoContent();
            }

            return NotFound($"Loan with number {loanNumber} not found");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting loan {LoanNumber}", loanNumber);
            return StatusCode(500, "An error occurred while deleting the loan");
        }
    }
}
