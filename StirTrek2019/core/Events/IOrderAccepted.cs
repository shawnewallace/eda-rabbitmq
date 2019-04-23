using System;
using System.Collections.Generic;

namespace eda.core.Events
{
	public interface IOrderAccepted : ICustomerId, IOrderId
	{
		List<OrderItem> OrderItems { get; set; }
	}
}