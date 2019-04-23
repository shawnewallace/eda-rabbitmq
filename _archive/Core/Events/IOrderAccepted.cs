using System;
using System.Collections.Generic;

namespace Core.Events
{
	public interface IOrderAccepted : ICustomerId, IOrderId
	{
		List<OrderItem> OrderItems { get; set; }
	}
}