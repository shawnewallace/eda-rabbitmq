using System;
using Core;

namespace Logger.Models
{
	public class OrderIdModel : IOrderId
	{
		public Guid OrderId { get; set; }
	}
}