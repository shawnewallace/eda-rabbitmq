using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using eda.core.data;
using Microsoft.EntityFrameworkCore;
using eda.core;
using RabbitMQ.Client.Events;
using RabbitMQ.Client;

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

public class DataService : BackgroundQService<DataService>
{
	private readonly LoggingContext _context;

	public DataService(LoggingContext context, ILogger<DataService> logger, IConfiguration configuration) : base(logger, configuration)
	{
		_context = context;
		Init();
	}

	private void Init()
	{
		Logger.LogInformation("[DATASERVICE] Init");

		LogginContextInitializer.Initialize(_context);

		var factory = GetConnectionFactory();
		Connection = factory.CreateConnection();

		Channel = Connection.CreateModel();

		DeclareExchange();
		DeclareQ(AppConstants.DATA_SERVICE_QUEUE_NAME);

		foreach (var eventName in AppConstants.EventCollection)
		{
			BindToQ(queueName: AppConstants.DATA_SERVICE_QUEUE_NAME,
								eventName: eventName);
		}
		SetUpQoS();

		Logger.LogInformation("[DATASERVICE] Init COMPLETE");
		Logger.LogInformation("[DATASERVICE] Waiting for messages.");
	}
	protected override Task ExecuteAsync(CancellationToken stoppingToken)
	{
		stoppingToken.ThrowIfCancellationRequested();

		var consumer = new EventingBasicConsumer(Channel);
		consumer.Received += (ch, ea) =>
		{
			// received message  
			var content = System.Text.Encoding.UTF8.GetString(ea.Body.Span);
			var routingKey = ea.RoutingKey;

			Logger.LogInformation($" [>>>>>>>>>>] DATASERVICE Received  '{routingKey}':'{content}'");

			// LogMessage(routingKey, content); -> DO WORK
			Channel.BasicAck(ea.DeliveryTag, false);
		};

		consumer.Shutdown += OnConsumerShutdown;
		consumer.Registered += OnConsumerRegistered;
		consumer.Unregistered += OnConsumerUnregistered;
		consumer.ConsumerCancelled += OnConsumerConsumerCancelled;

		Channel.BasicConsume(queue: AppConstants.DATA_SERVICE_QUEUE_NAME, autoAck: false, consumer: consumer);
		return Task.CompletedTask;
	}

	public override void Dispose()
	{
		Channel.Close();
		Connection.Close();
		base.Dispose();
	}
}