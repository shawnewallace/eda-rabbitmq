using System;
using eda.core.events;

namespace eda.shippingConsumer
{
  internal class CustomerBilledEvent : ICustomerBilled
  {
    public CustomerBilledEvent(Guid orderEvent) => OrderId = orderEvent;

    public Guid OrderId { get; set; }
  }
}
