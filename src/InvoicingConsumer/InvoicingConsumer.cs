using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using eda.core;
using eda.core.events;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace eda.invoicingConsumer
{
	internal class OrderShippedEvent : IOrderShipped
	{
		public Guid OrderId { get; set; }
		public DateTime Start { get; set; } = DateTime.UtcNow;
		public Guid EventId { get; set; } = Guid.NewGuid();
		public Guid CorrelationId { get; set; } = default!;
	}

	public class InvoicingConsumer : BackgroundQService<InvoicingConsumer>
	{
		private static int _ordersReceived = 0;

		public InvoicingConsumer(ILogger<InvoicingConsumer> logger, IConfiguration configuration) : base(logger, configuration)
		{
			Init();
		}

		private void Init()
		{
			Logger.LogInformation("[INVOICER] Init");

			var factory = GetConnectionFactory();
			//var factory = new ConnectionFactory { HostName = "localhost" };
			Connection = factory.CreateConnection();

			Channel = Connection.CreateModel();

			DeclareExchange();
			DeclareQ(AppConstants.INVOICING_QUEUE_NAME);
			BindToQ(queueName: AppConstants.INVOICING_QUEUE_NAME, eventName: AppConstants.ORDER_ACCEPTED_EVENT);
			BindToQ(queueName: AppConstants.INVOICING_QUEUE_NAME, eventName: AppConstants.SHIPPED_EVENT);
			SetUpQoS();

			Connection.ConnectionShutdown += RabbitMQ_ConnectionShutdown;

			Logger.LogInformation("[INVOICER] Init COMPLETE");
			Logger.LogInformation("[INVOICER] Waiting for messages.");
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

				Logger.LogInformation($" [>>>>>>>>>>] Received Order Accepted '{routingKey}':'{_ordersReceived++}'");

				// handle the received message  

				switch (routingKey)
				{
					case AppConstants.ORDER_ACCEPTED_EVENT:
						var orderAcceptedEvent = JsonConvert.DeserializeObject<Order>(content);
						ProcessEvent(orderAcceptedEvent);
						break;
					case AppConstants.SHIPPED_EVENT:
						var shippedEvent = JsonConvert.DeserializeObject<OrderShippedEvent>(content);
						var message = JsonConvert.SerializeObject(shippedEvent);
						var body = System.Text.Encoding.UTF8.GetBytes(message);
						RandomWait();
						Channel.BasicPublish(AppConstants.EXCHANGE_NAME, AppConstants.ORDER_FULLFILLED_EVENT, null, body);
						break;
				}

				Channel.BasicAck(ea.DeliveryTag, false);
			};

			consumer.Shutdown += OnConsumerShutdown;
			consumer.Registered += OnConsumerRegistered;
			consumer.Unregistered += OnConsumerUnregistered;
			consumer.ConsumerCancelled += OnConsumerConsumerCancelled;

			Channel.BasicConsume(queue: AppConstants.INVOICING_QUEUE_NAME, autoAck: false, consumer: consumer);
			return Task.CompletedTask;
		}

		private void ProcessEvent(IOrderAccepted orderEvent)
		{
			Logger.LogInformation("\tProcessing order {0}...", orderEvent.OrderId);
			ICustomerBilled billedEvent = new CustomerBilledEvent(orderEvent.OrderId, orderEvent.CorrelationId);
			RandomWait();
			var message = JsonConvert.SerializeObject(billedEvent);
			var body = System.Text.Encoding.UTF8.GetBytes(message);
			Channel.BasicPublish(AppConstants.EXCHANGE_NAME, AppConstants.CUSTOMER_BILLED_EVENT, null, body);

			Logger.LogInformation("Done");
		}

		private static IOrderAccepted DeserializeMessage(string message)
		{
			return JsonConvert.DeserializeObject<Order>(message);
		}

		public override void Dispose()
		{
			Channel.Close();
			Connection.Close();
			base.Dispose();
		}
	}
}
