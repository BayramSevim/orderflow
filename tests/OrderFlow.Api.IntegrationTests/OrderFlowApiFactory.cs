using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using OrderFlow.Infrastructure.Persistence;
using Testcontainers.PostgreSql;

namespace OrderFlow.Api.IntegrationTests
{
    public class OrderFlowApiFactory : WebApplicationFactory<Program>, IAsyncLifetime
    {
        private readonly PostgreSqlContainer _postgres = new PostgreSqlBuilder()
            .WithImage("postgres:17")
            .Build();

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureTestServices(services =>
            {
                services.RemoveAll<DbContextOptions<OrderFlowDbContext>>();
                services.AddDbContext<OrderFlowDbContext>(options =>
                    options.UseNpgsql(_postgres.GetConnectionString()));
            });
        }

        public Task InitializeAsync() => _postgres.StartAsync();

        public new Task DisposeAsync() => _postgres.DisposeAsync().AsTask();
    }
}