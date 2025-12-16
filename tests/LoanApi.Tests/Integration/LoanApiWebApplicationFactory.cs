using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using LoanApi.Data;
using LoanApi.Repositories;
using LoanApi.Services;

namespace LoanApi.Tests.Integration;

public class LoanApiWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // Remove the existing services
            var descriptors = services.Where(d =>
                d.ServiceType == typeof(DatabaseConfig) ||
                d.ServiceType == typeof(MasterConnectionKeeper) ||
                d.ServiceType == typeof(IDbConnectionFactory) ||
                d.ServiceType == typeof(ILoanRepository) ||
                d.ServiceType == typeof(DatabaseInitializer))
                .ToList();

            foreach (var descriptor in descriptors)
            {
                services.Remove(descriptor);
            }

            // Add test-specific services with unique database for each factory instance
            var uniqueConnectionString = $"Data Source=test_{Guid.NewGuid():N};Mode=Memory;Cache=Shared";
            services.AddSingleton(new DatabaseConfig(uniqueConnectionString));
            services.AddSingleton<MasterConnectionKeeper>();
            services.AddSingleton<IDbConnectionFactory, SqliteConnectionFactory>();
            services.AddSingleton<ILoanRepository, LoanRepository>();
            services.AddSingleton<DatabaseInitializer>();
            services.AddSingleton<LoanMappingService>();
        });
    }
}
