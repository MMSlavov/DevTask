# Loan API - DSK Credit App

REST API application for managing bank loans using in-memory SQLite database and Dapper for database queries.

## Features

- **In-memory SQLite database** - Fast and lightweight data storage
- **Dapper ORM** - Efficient database queries
- **RESTful API** - Standard HTTP methods for CRUD operations
- **Swagger UI** - Interactive API documentation

## Loan Data Model

Each loan contains the following information:
- **Loan Number** - Unique identifier for the loan
- **Client Name** - Name of the loan applicant
- **Amount** - Loan amount in decimal format
- **Request Date** - Date and time when the loan was requested
- **Status** - Current status of the loan (e.g., Pending, Approved, Rejected)

## API Endpoints

### Get All Loans
```
GET /api/loans
```
Returns a list of all loans in the system.

### Get Loan by Loan Number
```
GET /api/loans/{loanNumber}
```
Returns a specific loan by its loan number.

### Create New Loan
```
POST /api/loans
Content-Type: application/json

{
  "loanNumber": "LN-001",
  "clientName": "John Doe",
  "amount": 50000,
  "requestDate": "2025-12-16T10:00:00",
  "status": "Pending"
}
```
Creates a new loan record.

### Update Existing Loan
```
PUT /api/loans/{loanNumber}
Content-Type: application/json

{
  "loanNumber": "LN-001",
  "clientName": "John Doe",
  "amount": 55000,
  "requestDate": "2025-12-16T10:00:00",
  "status": "Approved"
}
```
Updates an existing loan record.

### Delete Loan
```
DELETE /api/loans/{loanNumber}
```
Deletes a loan from the system.

## Running the Application

### Prerequisites
- .NET 8.0 SDK or later

### Build and Run
```bash
cd LoanApi
dotnet restore
dotnet build
dotnet run
```

The API will be available at `http://localhost:5000` (or the port specified in launchSettings.json).

### Access Swagger UI
Once the application is running, navigate to:
```
http://localhost:5000/swagger
```

## Technology Stack

- **ASP.NET Core 8.0** - Web API framework
- **SQLite** - In-memory database
- **Dapper** - Micro ORM for database access
- **Swagger/OpenAPI** - API documentation

## Project Structure

```
LoanApi/
├── Controllers/
│   └── LoansController.cs     # REST API endpoints
├── Data/
│   ├── DatabaseInitializer.cs # Database setup and initialization
│   ├── ILoanRepository.cs     # Repository interface
│   └── LoanRepository.cs      # Repository implementation with Dapper
├── Models/
│   └── Loan.cs                # Loan data model
└── Program.cs                 # Application startup and configuration
```
