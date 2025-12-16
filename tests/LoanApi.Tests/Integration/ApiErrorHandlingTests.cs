using System.Net;

namespace LoanApi.Tests.Integration;

public class ApiErrorHandlingTests : IClassFixture<LoanApiWebApplicationFactory>
{
    private readonly HttpClient _client;

    public ApiErrorHandlingTests(LoanApiWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetLoans_WithInvalidRoute_ReturnsNotFound()
    {
        // Act
        var response = await _client.GetAsync("/api/invalid-route");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Theory]
    [InlineData("/api/loans")]
    [InlineData("/api/loans/stats")]
    public async Task ApiEndpoints_SupportMultipleConcurrentRequests(string endpoint)
    {
        // Arrange
        var tasks = Enumerable.Range(0, 10)
            .Select(_ => _client.GetAsync(endpoint))
            .ToArray();

        // Act
        var responses = await Task.WhenAll(tasks);

        // Assert
        responses.Should().AllSatisfy(r => r.IsSuccessStatusCode.Should().BeTrue());
    }
}
