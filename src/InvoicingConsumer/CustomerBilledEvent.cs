using System;
using eda.core.events;

namespace eda.invoicingConsumer
{
  internal class CustomerBilledEvent : ICustomerBilled
  {
    public CustomerBilledEvent(Guid orderEvent)
    {
      OrderId = orderEvent;
    }

    public Guid OrderId { get; set; }
  }
}
