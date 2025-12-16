using Microsoft.AspNetCore.Mvc;

namespace LoanApi.Tests.Controllers;

public class LoansControllerTests
{
    private readonly Mock<ILoanRepository> _mockRepository;
    private readonly LoanMappingService _mappingService;
    private readonly LoansController _controller;

    public LoansControllerTests()
    {
        _mockRepository = new Mock<ILoanRepository>();
        _mappingService = new LoanMappingService();
        _controller = new LoansController(_mockRepository.Object, _mappingService);
    }

    [Fact]
    public async Task GetLoans_ReturnsOkResultWithLoans()
    {
        // Arrange
        var loans = new List<Loan>
        {
            new()
            {
                LoanNumber = "LOAN001",
                ClientName = "John Doe",
                Amount = 10000m,
                RequestDate = new DateTime(2024, 1, 15),
                Status = LoanStatus.Paid,
                Invoices = new List<Invoice>
                {
                    new() { InvoiceNumber = "INV001", Amount = 5000m }
                }
            },
            new()
            {
                LoanNumber = "LOAN002",
                ClientName = "Jane Smith",
                Amount = 20000m,
                RequestDate = new DateTime(2024, 2, 20),
                Status = LoanStatus.AwaitingPayment,
                Invoices = new List<Invoice>()
            }
        };

        _mockRepository.Setup(r => r.GetAllAsync())
            .ReturnsAsync(loans);

        // Act
        var result = await _controller.GetLoans();

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedLoans = okResult.Value.Should().BeAssignableTo<IEnumerable<LoanResponse>>().Subject.ToList();
        
        returnedLoans.Should().HaveCount(2);
        returnedLoans[0].LoanNumber.Should().Be("LOAN001");
        returnedLoans[0].ClientName.Should().Be("John Doe");
        returnedLoans[0].Amount.Should().Be(10000m);
        returnedLoans[0].Status.Should().Be("Paid");
        returnedLoans[0].Invoices.Should().HaveCount(1);
        
        returnedLoans[1].LoanNumber.Should().Be("LOAN002");
        returnedLoans[1].ClientName.Should().Be("Jane Smith");
        returnedLoans[1].Amount.Should().Be(20000m);
        returnedLoans[1].Status.Should().Be("AwaitingPayment");
        returnedLoans[1].Invoices.Should().BeEmpty();

        _mockRepository.Verify(r => r.GetAllAsync(), Times.Once);
    }

    [Fact]
    public async Task GetLoans_WithNoLoans_ReturnsEmptyList()
    {
        // Arrange
        _mockRepository.Setup(r => r.GetAllAsync())
            .ReturnsAsync(new List<Loan>());

        // Act
        var result = await _controller.GetLoans();

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedLoans = okResult.Value.Should().BeAssignableTo<IEnumerable<LoanResponse>>().Subject;
        
        returnedLoans.Should().BeEmpty();
        _mockRepository.Verify(r => r.GetAllAsync(), Times.Once);
    }

    [Fact]
    public async Task GetLoanStats_ReturnsOkResultWithStats()
    {
        // Arrange
        var sumPaid = 60000m;
        var sumAwaiting = 40000m;

        _mockRepository.Setup(r => r.GetPaidAwaitingSumsAsync())
            .ReturnsAsync((sumPaid, sumAwaiting));

        // Act
        var result = await _controller.GetLoanStats();

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var stats = okResult.Value.Should().BeOfType<LoanStatsResponse>().Subject;
        
        stats.SumPaid.Should().Be(60000m);
        stats.SumAwaitingPayment.Should().Be(40000m);
        stats.PercentagePaid.Should().Be(60.00m);
        stats.PercentageAwaitingPayment.Should().Be(40.00m);

        _mockRepository.Verify(r => r.GetPaidAwaitingSumsAsync(), Times.Once);
    }

    [Fact]
    public async Task GetLoanStats_WithZeroAmounts_ReturnsZeroPercentages()
    {
        // Arrange
        _mockRepository.Setup(r => r.GetPaidAwaitingSumsAsync())
            .ReturnsAsync((0m, 0m));

        // Act
        var result = await _controller.GetLoanStats();

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var stats = okResult.Value.Should().BeOfType<LoanStatsResponse>().Subject;
        
        stats.SumPaid.Should().Be(0m);
        stats.SumAwaitingPayment.Should().Be(0m);
        stats.PercentagePaid.Should().Be(0m);
        stats.PercentageAwaitingPayment.Should().Be(0m);

        _mockRepository.Verify(r => r.GetPaidAwaitingSumsAsync(), Times.Once);
    }

    [Fact]
    public async Task GetLoanStats_WithOnlyPaidLoans_Returns100PercentPaid()
    {
        // Arrange
        _mockRepository.Setup(r => r.GetPaidAwaitingSumsAsync())
            .ReturnsAsync((50000m, 0m));

        // Act
        var result = await _controller.GetLoanStats();

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var stats = okResult.Value.Should().BeOfType<LoanStatsResponse>().Subject;
        
        stats.SumPaid.Should().Be(50000m);
        stats.SumAwaitingPayment.Should().Be(0m);
        stats.PercentagePaid.Should().Be(100.00m);
        stats.PercentageAwaitingPayment.Should().Be(0m);

        _mockRepository.Verify(r => r.GetPaidAwaitingSumsAsync(), Times.Once);
    }

    [Fact]
    public async Task GetLoanStats_WithOnlyAwaitingLoans_Returns100PercentAwaiting()
    {
        // Arrange
        _mockRepository.Setup(r => r.GetPaidAwaitingSumsAsync())
            .ReturnsAsync((0m, 75000m));

        // Act
        var result = await _controller.GetLoanStats();

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var stats = okResult.Value.Should().BeOfType<LoanStatsResponse>().Subject;
        
        stats.SumPaid.Should().Be(0m);
        stats.SumAwaitingPayment.Should().Be(75000m);
        stats.PercentagePaid.Should().Be(0m);
        stats.PercentageAwaitingPayment.Should().Be(100.00m);

        _mockRepository.Verify(r => r.GetPaidAwaitingSumsAsync(), Times.Once);
    }
}
