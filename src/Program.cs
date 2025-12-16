using LoanApi.Data;
using LoanApi.Repositories;
using LoanApi.Services;
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
builder.Services.AddSingleton<LoanMappingService>();

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

app.MapGet("/loans", async (ILoanRepository repository, LoanMappingService mappingService) =>
{
    var loans = await repository.GetAllAsync();
    var result = mappingService.MapLoansToResponses(loans);
    return Results.Ok(result);
})
.WithName("GetLoans");

app.MapGet("/loans/stats", async (ILoanRepository repository, LoanMappingService mappingService) =>
{
    var (sumPaid, sumAwaiting) = await repository.GetPaidAwaitingSumsAsync();
    var result = mappingService.MapStatsToResponse(sumPaid, sumAwaiting);
    return Results.Ok(result);
})
.WithName("GetLoanStats");

app.Run();
