﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core;
using Core.Events;
using Newtonsoft.Json;
using RabbitMQ.Client;

namespace SalesService
{
	public class OrderCreator : QueueBase
	{
		public void Submit(IOrderAccepted order)
		{
			var factory = GetConnectionFactory();
			using (var connection = factory.CreateConnection())
			{
				using (var channel = connection.CreateModel())
				{
					DeclareExchange(channel);

					var message = JsonConvert.SerializeObject(order);
					var body = Encoding.UTF8.GetBytes(message);

					channel.BasicPublish(Constants.EXCHANGE_NAME, Constants.ORDER_ACCEPTED_EVENT, null, body);
					Console.WriteLine(" [x] Sent '{0}':'{1}'", Constants.ORDER_ACCEPTED_EVENT, message);
				}
			}
		}
	}

	public class OrderAcceptedEvent : IOrderAccepted
	{
		public Guid CustomerId { get; set; }
		public Guid OrderId { get; set; }
		public List<OrderItem> OrderItems { get; set; }
	}
}