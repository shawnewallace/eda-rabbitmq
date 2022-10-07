using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using eda.core.data;
using Microsoft.EntityFrameworkCore;

namespace eda.dataService;

public class Program
{
	public static async Task<int> Main(string[] args)
	{
		try
		{
			var host = CreateHostBuilder(args);
			await host.RunConsoleAsync();
			return Environment.ExitCode;
		}
		catch (System.Exception)
		{
			
			throw;
		}
	}

	public static IHostBuilder CreateHostBuilder(string[] args) =>
		Host.CreateDefaultBuilder(args)
		.ConfigureLogging(logging =>
		{
			logging.ClearProviders();
			logging.AddConsole();
		})
		.ConfigureAppConfiguration((hostContext, builder) =>
		{
			builder.AddJsonFile("appsettings.json");
			builder.AddEnvironmentVariables();
			if (hostContext.HostingEnvironment.IsDevelopment())
			{
				builder.AddUserSecrets<Program>();
			}
		})
		.ConfigureServices((hostContext, services) =>
		{
			var connectionString = hostContext.Configuration["LogDatabaseConnectionString"];
			services.AddDbContext<LoggingContext>(x => x.UseSqlServer(connectionString));
			services.AddHostedService<DataService>();
		});
}
