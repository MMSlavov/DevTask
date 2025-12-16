using LoanApi.Data;
using Microsoft.Data.Sqlite;

var builder = WebApplication.CreateBuilder(args);

// Configure connection string for in-memory SQLite database
var connectionString = "Data Source=:memory:;Cache=Shared";

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Initialize the database and keep the connection open for in-memory database
var dbConnection = DatabaseInitializer.InitializeDatabase(connectionString);

// Register the connection and repository as singletons to keep the in-memory database alive
builder.Services.AddSingleton(dbConnection);
builder.Services.AddSingleton<ILoanRepository>(sp => new LoanRepository(dbConnection));

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
