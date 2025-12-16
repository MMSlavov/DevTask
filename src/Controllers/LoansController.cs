using LoanApi.Models.Responses;
using LoanApi.Repositories;
using LoanApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace LoanApi.Controllers;

[ApiController]
[Route("api/loans")]
public class LoansController : ControllerBase
{
    private readonly ILoanRepository _repository;
    private readonly LoanMappingService _mappingService;

    public LoansController(ILoanRepository repository, LoanMappingService mappingService)
    {
        _repository = repository;
        _mappingService = mappingService;
    }

    [HttpGet]
    [EndpointName("GetLoans")]
    public async Task<ActionResult<IEnumerable<LoanResponse>>> GetLoans()
    {
        var loans = await _repository.GetAllAsync();
        var result = _mappingService.MapLoansToResponses(loans);
        return Ok(result);
    }

    [HttpGet("stats")]
    [EndpointName("GetLoanStats")]
    public async Task<ActionResult<LoanStatsResponse>> GetLoanStats()
    {
        var (sumPaid, sumAwaiting) = await _repository.GetPaidAwaitingSumsAsync();
        var result = _mappingService.MapStatsToResponse(sumPaid, sumAwaiting);
        return Ok(result);
    }
}
