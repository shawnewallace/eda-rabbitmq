using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core;
using Core.Events;
using NUnit.Framework;

namespace SalesService.Unit.Tests
{
	[TestFixture]
	public class TestFixture1
	{
		[Test]
		public void Test1()
		{
			var service = new OrderCreator();
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
		}
	}
}
