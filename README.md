# Loan API

Minimal ASP.NET Core Web API that manages bank loans using an in-memory SQLite database and Dapper for data access. The service provides endpoints to retrieve loan information and statistics. Each loan includes: Loan number, Client name, Amount, Request date, Status, and associated Invoices.

## Technologies

- .NET 10.0
- ASP.NET Core Web API
- Dapper (micro ORM for data access)
- Microsoft.Data.Sqlite (in-memory database)
- Scalar.AspNetCore (API documentation)

## Project Structure

```
src/
├── Controllers/         # API endpoints
├── Data/               # Database configuration and initialization
├── Models/             # Domain models and response DTOs
├── Repositories/       # Data access layer
└── Services/           # Business logic and mapping

tests/
└── LoanApi.Tests/      # Unit and integration tests
```

## Database Schema

The application uses two tables in an in-memory SQLite database:

### Loans Table
| Column | Type | Description |
|--------|------|-------------|
| LoanNumber | TEXT (PK) | Unique loan identifier |
| ClientName | TEXT | Name of the client |
| Amount | REAL | Loan amount |
| RequestDate | TEXT | ISO 8601 formatted date when loan was requested |
| Status | TEXT | Loan status (Paid, AwaitingPayment, Created) |

### Invoices Table
| Column | Type | Description |
|--------|------|-------------|
| InvoiceNumber | TEXT (PK) | Unique invoice identifier |
| LoanNumber | TEXT (FK) | References Loans.LoanNumber |
| Amount | REAL | Invoice amount |

## Testing

The project includes comprehensive test coverage:

### Unit Tests
- **LoanMappingServiceTests** - Tests for business logic and DTO mapping
- **LoansControllerTests** - Controller behavior with mocked dependencies

### Integration Tests
- **LoansApiIntegrationTests** - End-to-end API endpoint testing
- **ApiErrorHandlingTests** - Error scenarios and edge cases

### Test Frameworks
- xUnit (test runner)
- Moq (mocking framework)
- FluentAssertions (assertion library)
- Microsoft.AspNetCore.Mvc.Testing (integration testing)

## Notes

- The SQLite database runs purely in memory; data resets on application restart but is kept alive during runtime via a master connection.
- Seed data is automatically loaded on startup for quick testing.
- Loan statuses supported: `Paid`, `AwaitingPayment`, `Created`.
- The stats endpoint calculates both absolute sums and percentages for Paid and AwaitingPayment loans.
