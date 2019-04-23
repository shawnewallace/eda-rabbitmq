using System;
using System.Collections.Generic;
using eda.core;
using eda.core.Events;

namespace eda.SalesService
{
  public class OrderAcceptedEvent : IOrderAccepted
  {
    public Guid CustomerId { get; set; }
    public Guid OrderId { get; set; }
    public List<OrderItem> OrderItems { get; set; }
  }
}