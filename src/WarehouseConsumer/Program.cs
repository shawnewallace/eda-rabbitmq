using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;


namespace eda.warehouseConsumer;
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
		catch
		{
			return 1;
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
			services.AddHostedService<WarehouseConsumer>();
		});
}
