using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Core;
using Core.Events;
using Microsoft.CSharp;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Framing.Impl;

namespace ShippingService
{
	class Shipper : QueueBase
	{
		static void Main(string[] args)
		{
			var factory = GetConnectionFactory();
			using (var connection = factory.CreateConnection())
			{
				using (var channel = connection.CreateModel())
				{
					DeclareExchange(channel);
					DeclareQ(channel, Constants.SHIPPING_QUEUE_NAME);
					SetUpQoS(channel);

					BindToQ(channel, Constants.SHIPPING_QUEUE_NAME, Constants.CUSTOMER_BILLED_EVENT);
					BindToQ(channel, Constants.SHIPPING_QUEUE_NAME, Constants.ORDER_ACCEPTED_EVENT);

					Console.WriteLine(" [SHIPPER] Waiting for messages.");

					var consumer = new EventingBasicConsumer(channel);
					consumer.Received += (Model, ea) =>
					{
						var body = ea.Body;
						var message = Encoding.UTF8.GetString(body);
						var routingKey = ea.RoutingKey;

						Console.WriteLine(" [>>>>>>>>>>] Received MESSAGE '{0}'", routingKey);

						ProcessMessage(channel, message, routingKey);
						channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
					};
					channel.BasicConsume(queue: Constants.SHIPPING_QUEUE_NAME, noAck: false, consumer: consumer);

					Console.WriteLine(" Press [enter] to exit.");
					Console.ReadLine();
				}
			}
		}

		private static void ProcessMessage(IModel channel, string message, string routingKey)
		{
			switch (routingKey)
			{
				case Constants.ORDER_ACCEPTED_EVENT:
					ProcessOrderAccepted(channel, message);
					return;
				case Constants.CUSTOMER_BILLED_EVENT:
					ProcessCustomerBilled(channel, message);
					return;
			}

			Console.WriteLine("***** UNABLE TO PROCESS MESSAGE {0} : {1}", routingKey, message);

			//throw new ArgumentException("Could not process message for routing key {0}", routingKey);
		}

		private static void ProcessCustomerBilled(IModel channel, string message)
		{
			var billed = JsonConvert.DeserializeObject<CustomerBilledEvent>(message);
			Console.Write(" [>>>>>>>>>>] Received Customer Billed '{0}'...", billed.OrderId);
			Thread.Sleep(10000);
			IOrderReadyForShipment ready = new OrderReady { OrderId = billed.OrderId};
			var orderMessage = JsonConvert.SerializeObject(billed);
			var body = Encoding.UTF8.GetBytes(orderMessage);
			channel.BasicPublish(Constants.EXCHANGE_NAME, Constants.READY_FOR_SHIPMENT_EVENT, null, body);
			Console.WriteLine("Processed");
		}

		private static void ProcessOrderAccepted(IModel channel, string message)
		{
			var order = JsonConvert.DeserializeObject<Order>(message);
			Console.Write(" [>>>>>>>>>>] Received Order Accepted '{0}'...", order.OrderId);
			Thread.Sleep(500);
			Console.WriteLine("Processed");
		}
	}

	internal class CustomerBilledEvent : ICustomerBilled
	{
		public CustomerBilledEvent(Guid orderEvent)
		{
			OrderId = orderEvent;
		}

		public Guid OrderId { get; set; }
	}

	internal class Order : IOrderAccepted
	{
		public Guid CustomerId { get; set; }
		public Guid OrderId { get; set; }
		public List<OrderItem> OrderItems { get; set; }
	}

	internal class OrderReady : IOrderReadyForShipment
	{
		public Guid OrderId { get; set; }
	}
}
