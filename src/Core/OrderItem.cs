using System;

namespace eda.core
{
  public class OrderItem : IOrderItem
  {
    public Guid ItemId { get; set; }
    public string Description { get; set; } = null!;
    public int Quantity { get; set; }
    public double Price { get; set; }
  }
}
