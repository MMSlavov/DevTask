using LoanApi.Data;
using LoanApi.Repositories;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

const string connectionString = "Data Source=:memory:;Mode=Memory;Cache=Shared";

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();
builder.Services.AddProblemDetails();

builder.Services.AddSingleton(new DatabaseConfig(connectionString));
builder.Services.AddSingleton<MasterConnectionKeeper>();
builder.Services.AddSingleton<IDbConnectionFactory, SqliteConnectionFactory>();
builder.Services.AddSingleton<ILoanRepository, LoanRepository>();
builder.Services.AddSingleton<DatabaseInitializer>();

var app = builder.Build();

await using (var scope = app.Services.CreateAsyncScope())
{
    var initializer = scope.ServiceProvider.GetRequiredService<DatabaseInitializer>();
    await initializer.InitializeAsync();
}

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseExceptionHandler("/error");
app.UseHttpsRedirection();

app.MapGet("/error", () => Results.Problem("An error occurred."))
    .ExcludeFromDescription();

app.MapGet("/loans", async (ILoanRepository repository) =>
{
    var loans = await repository.GetAllAsync();
    var result = loans.Select(loan => new
    {
        loan.LoanNumber,
        loan.ClientName,
        loan.Amount,
        loan.RequestDate,
        Status = loan.Status.ToString(),
        Invoices = loan.Invoices.Select(i => new { i.InvoiceNumber, i.Amount })
    });
    return Results.Ok(result);
})
.WithName("GetLoans");

app.MapGet("/loans/stats", async (ILoanRepository repository) =>
{
    var (sumPaid, sumAwaiting) = await repository.GetPaidAwaitingSumsAsync();
    var total = sumPaid + sumAwaiting;
    var pctPaid = total == 0 ? 0 : sumPaid / total * 100m;
    var pctAwaiting = total == 0 ? 0 : sumAwaiting / total * 100m;

    var result = new
    {
        SumPaid = sumPaid,
        SumAwaitingPayment = sumAwaiting,
        PercentagePaid = decimal.Round(pctPaid, 2),
        PercentageAwaitingPayment = decimal.Round(pctAwaiting, 2)
    };
    return Results.Ok(result);
})
.WithName("GetLoanStats");

app.Run();
