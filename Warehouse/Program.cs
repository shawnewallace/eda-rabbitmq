using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Core;
using Core.Events;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Warehouse
{
	class Warehouse : QueueBase
	{
		static void Main(string[] args)
		{
			var factory = GetConnectionFactory();
			using (var connection = factory.CreateConnection())
			{
				using (var channel = connection.CreateModel())
				{
					DeclareExchange(channel);
					DeclareQ(channel, Constants.WAREHOUSE_QUEUE_NAME);
					SetUpQoS(channel);

					BindToQ(channel, Constants.WAREHOUSE_QUEUE_NAME, Constants.READY_FOR_SHIPMENT_EVENT);

					Console.WriteLine(" [WAREHOUSE] Waiting for messages.");

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
					channel.BasicConsume(queue: Constants.WAREHOUSE_QUEUE_NAME, noAck: false, consumer: consumer);

					Console.WriteLine(" Press [enter] to exit.");
					Console.ReadLine();
				}
			}
		}

		private static void ProcessMessage(IModel channel, string message, string routingKey)
		{
			switch (routingKey)
			{
				case Constants.READY_FOR_SHIPMENT_EVENT:
					ProcessCustomerBilled(channel, message);
					return;
			}

			Console.WriteLine("***** UNABLE TO PROCESS MESSAGE {0} : {1}", routingKey, message);

			//throw new ArgumentException("Could not process message for routing key {0}", routingKey);
		}

		private static void ProcessCustomerBilled(IModel channel, string message)
		{
			var ready = JsonConvert.DeserializeObject<OrderReady>(message);
			Console.Write(" [>>>>>>>>>>] Received Order Ready For Shipment message '{0}'...", ready.OrderId);
			Thread.Sleep(15000);
			IOrderShipped shipped = new OrderShipped { OrderId = ready.OrderId };
			var orderMessage = JsonConvert.SerializeObject(ready);
			var body = Encoding.UTF8.GetBytes(orderMessage);
			channel.BasicPublish(Constants.EXCHANGE_NAME, Constants.SHIPPED_EVENT, null, body);
			Console.WriteLine("Shipped");
		}
	}

	internal class OrderShipped : IOrderShipped
	{
		public Guid OrderId { get; set; }
	}

	internal class OrderReady : IOrderReadyForShipment
	{
		public Guid OrderId { get; set; }
	}
}
