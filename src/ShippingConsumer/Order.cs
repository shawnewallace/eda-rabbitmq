using System;
using System.Collections.Generic;
using eda.core;
using eda.core.events;

namespace eda.shippingConsumer
{
  internal class Order : IOrderAccepted
  {
    public Guid CustomerId { get; set; }
    public Guid OrderId { get; set; }
    public List<OrderItem> OrderItems { get; set; }
  }
}
