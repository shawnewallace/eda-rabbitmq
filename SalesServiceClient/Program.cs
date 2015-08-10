using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Core;
using Core.Events;
using SalesService;

namespace SalesServiceClient
{
	class Program
	{
		static void Main(string[] args)
		{
			Console.WriteLine("hit enter to start");
			Console.ReadLine();


			var service = new OrderCreator();

			for (var i = 0; i < 100; i++)
			{
				IOrderAccepted order = new OrderAcceptedEvent();

				order.CustomerId = Guid.NewGuid();
				order.OrderId = Guid.NewGuid();
				order.OrderItems = new List<OrderItem>
				{
					new OrderItem { Description = "Item One", ItemId = Guid.NewGuid(), Price = 100.21, Quantity = 4 },
					new OrderItem { Description = "Item Two", ItemId = Guid.NewGuid(), Price = 19.99, Quantity = 6 },
					new OrderItem { Description = "Item Three", ItemId = Guid.NewGuid(), Price = 5, Quantity = 1 }
				};

				service.Submit(order);
				//Thread.Sleep(1000);
			}
		}
	}

}
