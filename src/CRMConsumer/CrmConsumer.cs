using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using eda.core;
using eda.core.events;
using Microsoft.Extensions.Configuration;

namespace eda.crmConsumer;

public class CrmConsumer : BackgroundQService<CrmConsumer>
{
	private static int _newCustomerReceived = 0;

	public CrmConsumer(ILogger<CrmConsumer> logger, IConfiguration configuration) : base(logger,configuration)
	{
		Init();
	}

	private void Init()
	{
		Logger.LogInformation("[CRM] Init");

		var factory = GetConnectionFactory();
		Connection = factory.CreateConnection();

		Channel = Connection.CreateModel();

		DeclareExchange();
		DeclareQ(AppConstants.MASTER_CUSTOMER_QUEUE_NAME);
		BindToQ(queueName: AppConstants.MASTER_CUSTOMER_QUEUE_NAME,
							eventName: AppConstants.NEW_CUSTOMER_EVENT);
		SetUpQoS();

		Connection.ConnectionShutdown += RabbitMQ_ConnectionShutdown;

		Logger.LogInformation("[CRM] Init COMPLETE");
		Logger.LogInformation("[CRM] Waiting for messages.");
	}

	protected override Task ExecuteAsync(CancellationToken stoppingToken)
	{
		stoppingToken.ThrowIfCancellationRequested();

		var consumer = new EventingBasicConsumer(Channel);
		consumer.Received += (ch, ea) =>
		{
			// received message  
			var content = System.Text.Encoding.UTF8.GetString(ea.Body.Span);
			var orderEvent = DeserializeMessage(content);
			var routingKey = ea.RoutingKey;

			Logger.LogInformation($" [>>>>>>>>>>] Received '{routingKey}':'{_newCustomerReceived++}'");

			// handle the received message  
			ProcessEvent(orderEvent);
			Channel.BasicAck(ea.DeliveryTag, false);
		};

		consumer.Shutdown += OnConsumerShutdown;
		consumer.Registered += OnConsumerRegistered;
		consumer.Unregistered += OnConsumerUnregistered;
		consumer.ConsumerCancelled += OnConsumerConsumerCancelled;

		Channel.BasicConsume(queue: AppConstants.MASTER_CUSTOMER_QUEUE_NAME, autoAck: false, consumer: consumer);
		return Task.CompletedTask;
	}

	private void ProcessEvent(INewCustomer newCustomer)
	{
		Logger.LogInformation("\tProcessing customer {0}...", newCustomer.CustomerId);
		RandomWait();

		var message = JsonConvert.SerializeObject(newCustomer);
		var body = System.Text.Encoding.UTF8.GetBytes(message);
		Channel.BasicPublish(AppConstants.EXCHANGE_NAME, AppConstants.CUSTOMER_CREATED_EVENT, null, body);

		Logger.LogInformation("Customer Created");
	}

	private static INewCustomer DeserializeMessage(string message)
	{
		return JsonConvert.DeserializeObject<NewCustomer>(message);
	}

	public override void Dispose()
	{
		Channel.Close();
		Connection.Close();
		base.Dispose();
	}
}

internal class NewCustomer : INewCustomer
{
	public Guid CustomerId { get; set; }

	public DateTime Start { get; set; } = DateTime.UtcNow;
	public Guid EventId { get; set; } = Guid.NewGuid();
	public Guid CorrelationId { get; set; } = default!;
}
