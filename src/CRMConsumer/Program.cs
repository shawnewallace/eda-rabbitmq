using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OpenTelemetry.Exporter;
using OpenTelemetry.Logs;
using OpenTelemetry.Resources;

namespace eda.crmConsumer;
public class Program
{
	public static async Task<int> Main(string[] args)
	{
		try{
			var host = CreateHostBuilder(args);

			host.ConfigureLogging(log =>
			{
				log.ClearProviders();
				log.AddOpenTelemetry(otel =>
				{
					otel.SetResourceBuilder(ResourceBuilder.CreateEmpty()
						.AddService("CRMConsumer"));
					otel.IncludeScopes = true;
					otel.IncludeFormattedMessage = true;
					
					otel.AddOtlpExporter(a =>
					{
						a.Endpoint = new Uri("http://localhost:5341/ingest/otlp/v1/logs");
						a.Protocol = OtlpExportProtocol.HttpProtobuf;
						// a.Headers = "X-Seq-ApiKey=WBPq4wjBhGll1QlL9m6r";
					});
				});
			});
			
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
		// .ConfigureLogging(logging =>
		// {
		// 	logging.ClearProviders();
		// 	logging.AddConsole();
		// })
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
			services.AddHostedService<CrmConsumer>();
		});
}
