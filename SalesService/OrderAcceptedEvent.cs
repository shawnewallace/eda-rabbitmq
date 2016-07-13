using System;
using System.Collections.Generic;
using Core;
using Core.Events;

namespace SalesService
{
  public class OrderAcceptedEvent : IOrderAccepted
  {
    public Guid CustomerId { get; set; }
    public Guid OrderId { get; set; }
    public List<OrderItem> OrderItems { get; set; }
  }
}