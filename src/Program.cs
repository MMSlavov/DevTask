using LoanApi.Data;
using LoanApi.Repositories;

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
}

app.UseExceptionHandler("/error");
app.UseHttpsRedirection();

app.MapGet("/error", () => Results.Problem("An error occurred."))
    .ExcludeFromDescription();

app.MapGet("/loans", async (ILoanRepository repository) =>
{
    var loans = await repository.GetAllAsync();
    return Results.Ok(loans);
})
.WithName("GetLoans");

app.Run();