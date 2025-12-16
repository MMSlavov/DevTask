# Loan API

Minimal ASP.NET Core Web API that manages bank loans using an in-memory SQLite database and Dapper for data access. The service exposes CRUD endpoints for loans with fields: Loan number, Client name, Amount, Request date, and Status.

## Getting started

1. Navigate to the project folder:
	 - `cd LoanApi`
2. Restore and run:
	 - `dotnet restore`
	 - `dotnet run`
3. The API starts on `https://localhost:5143` (or the port shown in the output). Swagger UI is available in development at `/swagger`.

## Notes

- The SQLite database runs purely in memory; data resets on application restart but is kept alive during runtime via a master connection.
- Seed data is loaded on startup for quick testing (loan numbers `LN-1001` to `LN-1003`).
