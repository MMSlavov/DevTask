using System.Net;
using System.Net.Http.Json;

namespace LoanApi.Tests.Integration;

public class LoansApiIntegrationTests : IClassFixture<LoanApiWebApplicationFactory>
{
    private readonly HttpClient _client;

    public LoansApiIntegrationTests(LoanApiWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetLoans_ReturnsSuccessStatusCode()
    {
        // Act
        var response = await _client.GetAsync("/api/loans");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetLoans_ReturnsJsonContent()
    {
        // Act
        var response = await _client.GetAsync("/api/loans");

        // Assert
        response.Content.Headers.ContentType?.MediaType.Should().Be("application/json");
    }

    [Fact]
    public async Task GetLoans_ReturnsListOfLoans()
    {
        // Act
        var loans = await _client.GetFromJsonAsync<List<LoanResponse>>("/api/loans");

        // Assert
        loans.Should().NotBeNull();
        loans.Should().BeOfType<List<LoanResponse>>();
    }

    [Fact]
    public async Task GetLoans_ReturnsLoansWithExpectedStructure()
    {
        // Act
        var loans = await _client.GetFromJsonAsync<List<LoanResponse>>("/api/loans");

        // Assert
        loans.Should().NotBeNull();

        if (loans!.Count > 0)
        {
            var firstLoan = loans[0];
            firstLoan.LoanNumber.Should().NotBeNullOrEmpty();
            firstLoan.ClientName.Should().NotBeNullOrEmpty();
            firstLoan.Amount.Should().BeGreaterThan(0);
            firstLoan.Status.Should().NotBeNullOrEmpty();
            firstLoan.Invoices.Should().NotBeNull();
        }
    }

    [Fact]
    public async Task GetLoanStats_ReturnsSuccessStatusCode()
    {
        // Act
        var response = await _client.GetAsync("/api/loans/stats");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetLoanStats_ReturnsJsonContent()
    {
        // Act
        var response = await _client.GetAsync("/api/loans/stats");

        // Assert
        response.Content.Headers.ContentType?.MediaType.Should().Be("application/json");
    }

    [Fact]
    public async Task GetLoanStats_ReturnsStatsWithExpectedStructure()
    {
        // Act
        var stats = await _client.GetFromJsonAsync<LoanStatsResponse>("/api/loans/stats");

        // Assert
        stats.Should().NotBeNull();
        stats!.SumPaid.Should().BeGreaterThanOrEqualTo(0);
        stats.SumAwaitingPayment.Should().BeGreaterThanOrEqualTo(0);
        stats.PercentagePaid.Should().BeInRange(0, 100);
        stats.PercentageAwaitingPayment.Should().BeInRange(0, 100);
    }

    [Fact]
    public async Task GetLoanStats_PercentagesShouldAddUpTo100()
    {
        // Act
        var stats = await _client.GetFromJsonAsync<LoanStatsResponse>("/api/loans/stats");

        // Assert
        stats.Should().NotBeNull();

        var total = stats!.PercentagePaid + stats.PercentageAwaitingPayment;

        if (stats.SumPaid > 0 || stats.SumAwaitingPayment > 0)
        {
            total.Should().BeApproximately(100m, 0.02m);
        }
        else
        {
            total.Should().Be(0m);
        }
    }

    [Fact]
    public async Task GetLoanStats_SumsMatchLoanData()
    {
        // Arrange - Get all loans first
        var loans = await _client.GetFromJsonAsync<List<LoanResponse>>("/api/loans");

        // Calculate expected sums
        decimal expectedPaid = 0;
        decimal expectedAwaiting = 0;

        if (loans != null)
        {
            foreach (var loan in loans)
            {
                if (loan.Status == "Paid")
                {
                    expectedPaid += loan.Amount;
                }
                else if (loan.Status == "AwaitingPayment")
                {
                    expectedAwaiting += loan.Amount;
                }
            }
        }

        // Act
        var stats = await _client.GetFromJsonAsync<LoanStatsResponse>("/api/loans/stats");

        // Assert
        stats.Should().NotBeNull();
        stats!.SumPaid.Should().Be(expectedPaid);
        stats.SumAwaitingPayment.Should().Be(expectedAwaiting);
    }

    [Fact]
    public async Task GetLoans_InvoicesHaveValidStructure()
    {
        // Act
        var loans = await _client.GetFromJsonAsync<List<LoanResponse>>("/api/loans");

        // Assert
        loans.Should().NotBeNull();

        foreach (var loan in loans!)
        {
            foreach (var invoice in loan.Invoices)
            {
                invoice.InvoiceNumber.Should().NotBeNullOrEmpty();
                invoice.Amount.Should().BeGreaterThan(0);
            }
        }
    }

    [Fact]
    public async Task GetLoans_RequestDateIsValid()
    {
        // Act
        var loans = await _client.GetFromJsonAsync<List<LoanResponse>>("/api/loans");

        // Assert
        loans.Should().NotBeNull();

        foreach (var loan in loans!)
        {
            loan.RequestDate.Should().BeBefore(DateTime.UtcNow.AddDays(1));
            loan.RequestDate.Should().BeAfter(DateTime.MinValue);
        }
    }
}
