namespace LoanApi.Tests.Services;

public class LoanMappingServiceTests
{
    private readonly LoanMappingService _sut;

    public LoanMappingServiceTests()
    {
        _sut = new LoanMappingService();
    }

    [Fact]
    public void MapLoansToResponses_WithValidLoans_ReturnsMappedResponses()
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
                    new() { InvoiceNumber = "INV001", Amount = 5000m },
                    new() { InvoiceNumber = "INV002", Amount = 5000m }
                }
            },
            new()
            {
                LoanNumber = "LOAN002",
                ClientName = "Jane Smith",
                Amount = 20000m,
                RequestDate = new DateTime(2024, 2, 20),
                Status = LoanStatus.AwaitingPayment,
                Invoices = new List<Invoice>
                {
                    new() { InvoiceNumber = "INV003", Amount = 20000m }
                }
            }
        };

        // Act
        var result = _sut.MapLoansToResponses(loans).ToList();

        // Assert
        result.Should().HaveCount(2);
        
        result[0].LoanNumber.Should().Be("LOAN001");
        result[0].ClientName.Should().Be("John Doe");
        result[0].Amount.Should().Be(10000m);
        result[0].RequestDate.Should().Be(new DateTime(2024, 1, 15));
        result[0].Status.Should().Be("Paid");
        result[0].Invoices.Should().HaveCount(2);
        result[0].Invoices.First().InvoiceNumber.Should().Be("INV001");
        result[0].Invoices.First().Amount.Should().Be(5000m);

        result[1].LoanNumber.Should().Be("LOAN002");
        result[1].ClientName.Should().Be("Jane Smith");
        result[1].Amount.Should().Be(20000m);
        result[1].Status.Should().Be("AwaitingPayment");
        result[1].Invoices.Should().HaveCount(1);
    }

    [Fact]
    public void MapLoansToResponses_WithEmptyList_ReturnsEmptyList()
    {
        // Arrange
        var loans = new List<Loan>();

        // Act
        var result = _sut.MapLoansToResponses(loans);

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public void MapLoansToResponses_WithLoanWithoutInvoices_ReturnsResponseWithEmptyInvoices()
    {
        // Arrange
        var loans = new List<Loan>
        {
            new()
            {
                LoanNumber = "LOAN003",
                ClientName = "Bob Johnson",
                Amount = 15000m,
                RequestDate = new DateTime(2024, 3, 10),
                Status = LoanStatus.Created,
                Invoices = new List<Invoice>()
            }
        };

        // Act
        var result = _sut.MapLoansToResponses(loans).ToList();

        // Assert
        result.Should().HaveCount(1);
        result[0].Invoices.Should().BeEmpty();
    }

    [Fact]
    public void MapStatsToResponse_WithValidSums_ReturnsCorrectPercentages()
    {
        // Arrange
        var sumPaid = 60000m;
        var sumAwaiting = 40000m;

        // Act
        var result = _sut.MapStatsToResponse(sumPaid, sumAwaiting);

        // Assert
        result.SumPaid.Should().Be(60000m);
        result.SumAwaitingPayment.Should().Be(40000m);
        result.PercentagePaid.Should().Be(60.00m);
        result.PercentageAwaitingPayment.Should().Be(40.00m);
    }

    [Fact]
    public void MapStatsToResponse_WithZeroTotal_ReturnsZeroPercentages()
    {
        // Arrange
        var sumPaid = 0m;
        var sumAwaiting = 0m;

        // Act
        var result = _sut.MapStatsToResponse(sumPaid, sumAwaiting);

        // Assert
        result.SumPaid.Should().Be(0m);
        result.SumAwaitingPayment.Should().Be(0m);
        result.PercentagePaid.Should().Be(0m);
        result.PercentageAwaitingPayment.Should().Be(0m);
    }

    [Fact]
    public void MapStatsToResponse_WithOnlyPaidAmount_Returns100PercentPaid()
    {
        // Arrange
        var sumPaid = 50000m;
        var sumAwaiting = 0m;

        // Act
        var result = _sut.MapStatsToResponse(sumPaid, sumAwaiting);

        // Assert
        result.SumPaid.Should().Be(50000m);
        result.SumAwaitingPayment.Should().Be(0m);
        result.PercentagePaid.Should().Be(100.00m);
        result.PercentageAwaitingPayment.Should().Be(0m);
    }

    [Fact]
    public void MapStatsToResponse_WithOnlyAwaitingAmount_Returns100PercentAwaiting()
    {
        // Arrange
        var sumPaid = 0m;
        var sumAwaiting = 75000m;

        // Act
        var result = _sut.MapStatsToResponse(sumPaid, sumAwaiting);

        // Assert
        result.SumPaid.Should().Be(0m);
        result.SumAwaitingPayment.Should().Be(75000m);
        result.PercentagePaid.Should().Be(0m);
        result.PercentageAwaitingPayment.Should().Be(100.00m);
    }

    [Fact]
    public void MapStatsToResponse_WithDecimalValues_RoundsToTwoDecimalPlaces()
    {
        // Arrange
        var sumPaid = 33333.33m;
        var sumAwaiting = 66666.67m;

        // Act
        var result = _sut.MapStatsToResponse(sumPaid, sumAwaiting);

        // Assert
        result.PercentagePaid.Should().Be(33.33m);
        result.PercentageAwaitingPayment.Should().Be(66.67m);
    }
}
