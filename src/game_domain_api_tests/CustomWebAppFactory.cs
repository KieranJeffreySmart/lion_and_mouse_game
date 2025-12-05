using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using game_domain_api.Repository;

namespace game_domain_api_tests;

public class CustomWebApplicationFactory<TProgram>: WebApplicationFactory<TProgram> where TProgram : class
{
    private readonly string dbName = $"testDb_{Guid.NewGuid()}";

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        Environment.SetEnvironmentVariable("DB_CONNECTION_TYPE", "inMemory");
        Environment.SetEnvironmentVariable("DB_CONNECTION_NAME", dbName);
        builder.ConfigureServices(s =>
        {
            s.AddDbContext<GameDbContext>(options =>
            {
                options.UseInMemoryDatabase(dbName);
            });
        });
    }
}