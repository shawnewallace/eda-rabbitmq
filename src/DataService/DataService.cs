using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using eda.core.data;
using eda.core;
using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using Azure.Storage.Blobs;
using System.Text;
using Newtonsoft.Json;

namespace eda.dataService;

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
		consumer.Received += async (ch, ea) =>
		{
			// received message  
			var content = System.Text.Encoding.UTF8.GetString(ea.Body.Span);
			var routingKey = ea.RoutingKey;

			Logger.LogInformation($" [>>>>>>>>>>] DATASERVICE Received  '{routingKey}':'{content}'");

			// LogMessage(routingKey, content); -> DO WORK

			await WriteEventToBlobStorage(routingKey, content);

			Channel.BasicAck(ea.DeliveryTag, false);
		};

		consumer.Shutdown += OnConsumerShutdown;
		consumer.Registered += OnConsumerRegistered;
		consumer.Unregistered += OnConsumerUnregistered;
		consumer.ConsumerCancelled += OnConsumerConsumerCancelled;

		Channel.BasicConsume(queue: AppConstants.DATA_SERVICE_QUEUE_NAME, autoAck: false, consumer: consumer);
		return Task.CompletedTask;
	}

	private async Task WriteEventToBlobStorage(string eventName, string content)
	{
		var blobStorageConnectionString = Configuration["BlobStorageConnectionString"];
		var blobStorageContainerName = Configuration["BlobStorageContainerName"];

		var evt = JsonConvert.DeserializeObject<EventWithCorrelationId>(content);
		var correlationId = evt is null ? "correlationIdIsNull" : evt.CorrelationId.ToString();
		var rightNowUtc = DateTime.UtcNow;
		var blobName = $"{rightNowUtc:yyyyMMddHHmmssfff}-{eventName}-{correlationId}.json";

		Logger.LogInformation($"Writing new event {eventName} to file {blobName}. CorrelationId is {correlationId}");

		var container = new BlobContainerClient(blobStorageConnectionString, blobStorageContainerName);
		var blob = container.GetBlobClient(blobName);

		using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(content)))
		{
			await blob.UploadAsync(ms);
		}
	}

	public override void Dispose()
	{
		Channel.Close();
		Connection.Close();
		base.Dispose();
	}
}
