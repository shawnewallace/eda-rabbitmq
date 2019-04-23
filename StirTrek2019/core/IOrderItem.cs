using System;

namespace eda.core
{
	public interface IOrderItem : IItem
	{
		string Description { get; set; }
		int Quantity { get; set; }
		double Price { get; set; }
	}

	public class OrderItem : IOrderItem
	{
		public Guid ItemId { get; set; }
		public string Description { get; set; }
		public int Quantity { get; set; }
		public double Price { get; set; }
	}
}