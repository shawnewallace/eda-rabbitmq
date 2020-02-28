using System;
using eda.core.events;

namespace eda.shippingConsumer
{
  internal class OrderReady : IOrderReadyForShipment
  {
    public Guid OrderId { get; set; }

  }
}
